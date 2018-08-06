using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
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
        private readonly ILearnerPerActorService _learnerPerActorService;
        private readonly ICache<IMessage> _messageCache;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;
        private readonly IFileDataCache _fileDataCache;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            ILearnerPerActorService learnerPerActorService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IFileDataCache fileDataCache,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService<U> validationOutputService,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _messageCache = messageCache;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _fileDataCache = fileDataCache;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _logger = logger;
        }

        public IEnumerable<U> Execute(IPreValidationContext validationContext)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // get ILR data from file
            _preValidationPopulationService.Populate();
            _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

            // get the learners
            var ilrMessage = _messageCache.Item;

            // Get L/A and split the learners into separate lists
            var messageShards = _learnerPerActorService.Process();
            stopWatch.Restart();

            var actorTasks = new List<Task<string>>();

            foreach (var messageShard in messageShards)
            {
                _logger.LogDebug($"validation Shard has {messageShard.Learners.Count} learners");

                // create actors for each Shard.
                var actor = GetValidationActor();

                // TODO:get reference data per each shard and send it to Actors
                var ilrMessageAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(messageShard));

                var internalDataCacheAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_internalDataCache));
                var externalDataCacheAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_externalDataCache));
                var fileDataCacheAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(_fileDataCache));

                var validationActorModel = new ValidationActorModel()
                {
                    JobId = validationContext.JobId,
                    Message = ilrMessageAsBytes,
                    InternalDataCache = internalDataCacheAsBytes,
                    ExternalDataCache = externalDataCacheAsBytes,
                    FileDataCache = fileDataCacheAsBytes,
                };

                actorTasks.Add(Task.Run(() => actor.Validate(validationActorModel)));
            }

            _logger.LogDebug($"Starting {actorTasks.Count} validation actors");

            Task.WaitAll(actorTasks.ToArray());

            _logger.LogDebug("all Actors completed");

            foreach (var actorTask in actorTasks)
            {
                var errors = _jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result);

                foreach (var error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }

            _logger.LogDebug($"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");
            _validationOutputService.Process();
            _logger.LogDebug($"Validation Final results persisted {stopWatch.ElapsedMilliseconds}");

            return null;
        }

        private IValidationActor GetValidationActor()
        {
            return ActorProxy.Create<IValidationActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"));
        }
    }
}
