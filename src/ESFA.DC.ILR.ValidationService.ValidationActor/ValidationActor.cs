using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ValidationService.ValidationActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    public class ValidationActor : Actor, IValidationActor
    {
        private ILifetimeScope _parentLifeTimeScope;
        private ActorId _actorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationActor"/> class.
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ValidationActor(ActorService actorService, ActorId actorId, ILifetimeScope parentLifeTimeScope)
            : base(actorService, actorId)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _actorId = actorId;

        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.StateManager.TryAddStateAsync("count", 5);
        }


        public Task<string> Validate(string jobId, IMessage message, ILearner[] shreddedLearners)
        {
            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope())
            {
                var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = jobId;
                var logger = childLifeTimeScope.Resolve<ILogger>();
                try
                {

                }
                catch (Exception ex)
                {
                    ActorEventSource.Current.ActorMessage(this, "Exception-{0}", ex.ToString());
                    logger.LogError("Error while processing Actor job", ex);
                    throw;
                }

            }
        }
    }
}
