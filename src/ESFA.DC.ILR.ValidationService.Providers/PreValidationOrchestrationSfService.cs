using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model;
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
        private IValidationItemProviderService<IInternalDataCache> _internalDataCacheProviderService;

        public PreValidationOrchestrationSfService(
            IPreValidationPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            ILearnerPerActorService<T, IEnumerable<ILearner>> learnerPerActorService,
            [KeyFilter("Json")] ISerializationService jsonSerializationService,
            IValidationItemProviderService<IInternalDataCache> internalDataCacheProviderService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _messageCache = messageCache;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCacheProviderService = internalDataCacheProviderService;
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
                var internalDataCacheAsBytes =
                    Encoding.UTF8.GetBytes(
                        _jsonSerializationService.Serialize(_internalDataCacheProviderService.Provide()));

                var validationActorModel = new ValidationActorModel()
                {
                    JobId = validationContext.JobId,
                    Message = ilrMessageAsBytes,
                    ShreddedLearners = learnersShardAsBytes,
                    InternalDataCache = internalDataCacheAsBytes
                };

                actorTasks.Add(Task.Run(() =>
                    actor.Validate(validationActorModel)));
            }

            Task.WaitAll(actorTasks.ToArray());
            var results = new List<U>();
            foreach (var actorTask in actorTasks)
            {
                results.AddRange(_jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result));
            }

            return results;
        }

        private IValidationActor GetValidationActor()
        {
            return ActorProxy.Create<IValidationActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"));
        }
    }
}
