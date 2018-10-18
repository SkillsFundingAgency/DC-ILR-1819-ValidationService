using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class PreValidationOrchestrationSfService<U> : IPreValidationOrchestrationService<U>
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly IErrorLookupPopulationService _errorLookupPopulationService;
        private readonly ILearnerPerActorService _learnerPerActorService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;
        private readonly IFileDataCache _fileDataCache;
        private readonly ICache<string> _cache;
        private readonly IMessageStreamProviderService _streamProvider;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _ruleSetOrchestrationService;
        private readonly ILogger _logger;
        private readonly IValidateXMLSchemaService _validateXmlSchemaService;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            IErrorLookupPopulationService errorLookupPopulationService,
            ILearnerPerActorService learnerPerActorService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IFileDataCache fileDataCache,
            ICache<string> cache,
            IMessageStreamProviderService streamProvider,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService<U> validationOutputService,
            IRuleSetOrchestrationService<IMessage, U> ruleSetOrchestrationService,
            ILogger logger,
            IValidateXMLSchemaService validateXMLSchemaService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _errorLookupPopulationService = errorLookupPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _fileDataCache = fileDataCache;
            _cache = cache;
            _streamProvider = streamProvider;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
            _logger = logger;
            _validateXmlSchemaService = validateXMLSchemaService;
        }

        public async Task ExecuteAsync(IPreValidationContext validationContext, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                // get ILR data from file
                await _errorLookupPopulationService.PopulateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Error lookup service completed in: {stopWatch.ElapsedMilliseconds}");

                // Todo: Remove this when XML is known to be schema valid
                Stream fileStream = await _streamProvider.Provide(cancellationToken);
                if (fileStream != null)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
                    using (StreamReader reader = new StreamReader(fileStream, utF8Encoding, true))
                    {
                        Cache<string> fileContentCache = (Cache<string>)_cache;
                        fileContentCache.Item = reader.ReadToEnd();
                    }
                }

                if (_validationErrorCache.ValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Call XSD validation
                _validateXmlSchemaService.Validate();
                _logger.LogDebug($"XML validation schema service completed in: {stopWatch.ElapsedMilliseconds}");

                if (_validationErrorCache.ValidationErrors.Any(IsErrorOrFail))
                {
                    _logger.LogDebug(
                        $"Possible xsd validation failure: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await _preValidationPopulationService.PopulateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

                // Set the filename
                _fileDataCache.FileName = validationContext.Input;

                // File Validation
                await _ruleSetOrchestrationService.Execute(cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsErrorOrFail))
                {
                    _logger.LogDebug(
                        $"Header validation failed, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await ExecuteValidationActors(validationContext, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug(
                    $"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");
            }
            catch (Exception ex)
            {
                _logger.LogError("Validation Critical Error", ex);
            }
            finally
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _validationOutputService.ProcessAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Validation final results persisted in {stopWatch.ElapsedMilliseconds}");
            }
        }

        private IValidationActor GetValidationActor()
        {
            return ActorProxy.Create<IValidationActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"));
        }

        private async Task DestroyValidationActorAsync(IValidationActor validationActor, CancellationToken cancellationToken)
        {
            try
            {
                ActorId actorId = validationActor.GetActorId();

                IActorService myActorServiceProxy = ActorServiceProxy.Create(
                    new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"),
                    actorId);

                await myActorServiceProxy.DeleteActorAsync(actorId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Problem deleting actor", ex);
            }
        }

        private async Task ExecuteValidationActors(IPreValidationContext validationContext, CancellationToken cancellationToken)
        {
            // Get L/A and split the learners into separate lists
            IEnumerable<IMessage> messageShards = _learnerPerActorService.Process();

            List<IValidationActor> actors = new List<IValidationActor>();
            List<Task<string>> actorTasks = new List<Task<string>>();
            List<Task> actorDestroys = new List<Task>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _logger.LogDebug($"Validation will create {messageShards.Count()} actors");

            string internalDataCacheAsString =
                _jsonSerializationService.Serialize(_internalDataCache);
            string externalDataCacheAsString =
                _jsonSerializationService.Serialize(_externalDataCache);
            string fileDataCacheAsString =
                _jsonSerializationService.Serialize(_fileDataCache);

            _logger.LogDebug($" actor will be given ExternalDataCache: {externalDataCacheAsString.Length} ");

            foreach (IMessage messageShard in messageShards)
            {
                _logger.LogDebug($"Validation Shard has {messageShard.Learners.Count} learners");
                _logger.LogDebug($" actor will be given Postcodes: {_externalDataCache.Postcodes.Count} : ULNs: {_externalDataCache.ULNs.Count} FCS: {_externalDataCache.FCSContracts.Count} EPA: {_externalDataCache.EPAOrganisations.Count} LARS: {_externalDataCache.LearningDeliveries.Count} Orgs: {_externalDataCache.Organisations}");

                // create actors for each Shard.
                IValidationActor actor = GetValidationActor();
                actors.Add(actor);

                // TODO:get reference data per each shard and send it to Actors
                string ilrMessageAsString = _jsonSerializationService.Serialize(messageShard);

                ValidationActorModel validationActorModel = new ValidationActorModel
                {
                    JobId = validationContext.JobId,
                    Message = ilrMessageAsString,
                    InternalDataCache = internalDataCacheAsString,
                    ExternalDataCache = externalDataCacheAsString,
                    FileDataCache = fileDataCacheAsString,
                };

                actorTasks.Add(actor.Validate(validationActorModel, cancellationToken));
            }

            _logger.LogDebug($"Starting {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms prep time");
            stopWatch.Restart();

            await Task.WhenAll(actorTasks.ToArray()).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug($"Collating {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms execution time");
            stopWatch.Restart();

            foreach (Task<string> actorTask in actorTasks)
            {
                IEnumerable<U> errors = _jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result);

                foreach (U error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }

            _logger.LogDebug($"Destroying {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms collation time");

            foreach (IValidationActor validationActor in actors)
            {
                actorDestroys.Add(DestroyValidationActorAsync(validationActor, cancellationToken));
            }

            await Task.WhenAll(actorDestroys.ToArray()).ConfigureAwait(false);
        }

        private bool IsErrorOrFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Error || severity == Severity.Fail;
        }
    }
}
