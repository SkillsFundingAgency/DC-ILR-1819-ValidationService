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

        private async Task ExecuteValidationActors(IPreValidationContext validationContext, CancellationToken cancellationToken)
        {
            // Get L/A and split the learners into separate lists
            IEnumerable<IMessage> messageShards = _learnerPerActorService.Process();

            List<Task<string>> actorTasks = new List<Task<string>>();

            foreach (IMessage messageShard in messageShards)
            {
                _logger.LogDebug($"validation Shard has {messageShard.Learners.Count} learners");

                // create actors for each Shard.
                IValidationActor actor = GetValidationActor();

                // TODO:get reference data per each shard and send it to Actors
                byte[] ilrMessageAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(messageShard));

                byte[] internalDataCacheAsBytes =
                    Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_internalDataCache));
                byte[] externalDataCacheAsBytes =
                    Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_externalDataCache));
                byte[] fileDataCacheAsBytes =
                    Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_fileDataCache));

                ValidationActorModel validationActorModel = new ValidationActorModel
                {
                    JobId = validationContext.JobId,
                    Message = ilrMessageAsBytes,
                    InternalDataCache = internalDataCacheAsBytes,
                    ExternalDataCache = externalDataCacheAsBytes,
                    FileDataCache = fileDataCacheAsBytes,
                };

                actorTasks.Add(actor.Validate(validationActorModel, cancellationToken));
            }

            _logger.LogDebug($"Starting {actorTasks.Count} validation actors");

            await Task.WhenAll(actorTasks.ToArray()).ConfigureAwait(false);

            _logger.LogDebug("All Validation Actors completed");

            cancellationToken.ThrowIfCancellationRequested();

            foreach (Task<string> actorTask in actorTasks)
            {
                IEnumerable<U> errors = _jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result);

                foreach (U error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }
        }

        private bool IsErrorOrFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Error || severity == Severity.Fail;
        }
    }
}
