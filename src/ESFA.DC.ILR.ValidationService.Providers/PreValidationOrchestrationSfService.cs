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
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _ruleSetOrchestrationService;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            IErrorLookupPopulationService errorLookupPopulationService,
            ILearnerPerActorService learnerPerActorService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IFileDataCache fileDataCache,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService<U> validationOutputService,
            IRuleSetOrchestrationService<IMessage, U> ruleSetOrchestrationService,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _errorLookupPopulationService = errorLookupPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _fileDataCache = fileDataCache;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
            _logger = logger;
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

                if (_validationErrorCache.ValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                await _preValidationPopulationService.PopulateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

                // Set the filename
                _fileDataCache.FileName = validationContext.Input;

                // File Validation
                await _ruleSetOrchestrationService.Execute(cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsFail))
                {
                    _logger.LogDebug(
                        $"File schema catestrophic error, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await ExecuteValidationActors(validationContext, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug(
                    $"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");
            }
            catch (Exception ex)
            {
                _logger.LogError("Validation Critical Error", ex);
                throw;
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
            _logger.LogDebug($"_internalDataCache {internalDataCacheAsString.Length}");
            string fileDataCacheAsString =
                _jsonSerializationService.Serialize(_fileDataCache);
            _logger.LogDebug($"fileDataCacheAsString {fileDataCacheAsString.Length}");
            string externalDataCacheAsString =
                _jsonSerializationService.Serialize(_externalDataCache);
            _logger.LogDebug($"ExternalDataCache: {externalDataCacheAsString.Length} ");

            foreach (IMessage messageShard in messageShards)
            {
                _logger.LogDebug($"Validation Shard has {messageShard.Learners.Count} learners");

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

        private bool IsFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Fail;
        }
    }
}
