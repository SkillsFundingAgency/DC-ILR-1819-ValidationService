using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class PreValidationOrchestrationSfService<T, U> : IPreValidationOrchestrationService<T, U>
        where T : class
    {
        private readonly IPreValidationPopulationService _preValidationPopulationService;
        private readonly ILearnerPerActorService<T, IEnumerable<ILearner>> _learnerPerActorService;
        private readonly ICache<IMessage> _messageCache;
        private readonly ISerializationService _jsonSerializationService;
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService<U> _validationOutputService;

        public PreValidationOrchestrationSfService(
            IPreValidationPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            ILearnerPerActorService<T, IEnumerable<ILearner>> learnerPerActorService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService<U> validationOutputService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _messageCache = messageCache;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IPreValidationContext validationContext)
        {
            // get ILR data from file
            _preValidationPopulationService.Populate();

            // get the learners
            var ilrMessage = _messageCache.Item;

            // Get L/A and split the learners into separate lists
            var learnerShards = _learnerPerActorService.Process();

            var actorTasks = new List<Task<string>>();

            foreach (var learnerShard in learnerShards)
            {
                // create actors for each Shard.
                var actor = GetValidationActor();

                // TODO:get reference data per each shard and send it to Actors
                var ilrMessageAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(ilrMessage));
                var learnersShardAsBytes = Encoding.UTF8.GetBytes(_jsonSerializationService.Serialize(learnerShard));
                var internalDataCache = _internalDataCache;
                var internalDataCacheString = _jsonSerializationService.Serialize(internalDataCache);
                var externalDataCache = _externalDataCache;
                var externalDataCacheString = _jsonSerializationService.Serialize(externalDataCache);

                var internalDataCacheAsBytes = Encoding.UTF8.GetBytes(internalDataCacheString);
                var externalDataCacheAsBytes = Encoding.UTF8.GetBytes(externalDataCacheString);

                var validationActorModel = new ValidationActorModel()
                {
                    JobId = validationContext.JobId,
                    Message = ilrMessageAsBytes,
                    ShreddedLearners = learnersShardAsBytes,
                    InternalDataCache = internalDataCacheAsBytes,
                    ExternalDataCache = externalDataCacheAsBytes,
                };

                actorTasks.Add(Task.Run(() =>
                    actor.Validate(validationActorModel)));
            }

            Task.WaitAll(actorTasks.ToArray());

            foreach (var actorTask in actorTasks)
            {
                var errors = _jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result);

                foreach (var error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }

            return _validationOutputService.Process();
        }

        private IValidationActor GetValidationActor()
        {
            return ActorProxy.Create<IValidationActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"));
        }
    }
}
