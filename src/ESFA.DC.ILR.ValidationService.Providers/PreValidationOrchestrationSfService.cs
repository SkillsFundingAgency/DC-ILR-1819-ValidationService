using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
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
        private readonly ISerializationService _serializationService;

        public PreValidationOrchestrationSfService(
            IPreValidationPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            ILearnerPerActorService<T, IEnumerable<ILearner>> learnerPerActorService,
            ISerializationService serializationService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerPerActorService = learnerPerActorService;
            _messageCache = messageCache;
            _serializationService = serializationService;
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
                actorTasks.Add(Task.Run(() =>
                    actor.Validate(validationContext.JobId, ilrMessage, learnerShard.ToArray())));
            }

            Task.WaitAll(actorTasks.ToArray());
            var results = new List<U>();
            foreach (var actorTask in actorTasks)
            {
                results.AddRange(_serializationService.Deserialize<IEnumerable<U>>(actorTask.Result));
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
