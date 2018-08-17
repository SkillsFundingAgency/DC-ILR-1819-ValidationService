using System.IO;
using System.Text;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;

namespace ESFA.DC.ILR.ValidationService.ValidationActor
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Stateless.Models;
    using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
    using ESFA.DC.Logging.Interfaces;
    using ESFA.DC.Serialization.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

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
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly ActorId _actorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationActor"/> class.
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="parentLifeTimeScope">Autofac Parent Lifetime Scope</param>
        public ValidationActor(ActorService actorService, ActorId actorId, ILifetimeScope parentLifeTimeScope)
            : base(actorService, actorId)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _actorId = actorId;
        }

        public Task<string> Validate(ValidationActorModel validationActorModel)
        {
            var jsonSerializationService = _parentLifeTimeScope.Resolve<IJsonSerializationService>();

            var internalDataCache = jsonSerializationService.Deserialize<InternalDataCache>(Encoding.UTF8.GetString(validationActorModel.InternalDataCache));
            var externalDataCache = jsonSerializationService.Deserialize<ExternalDataCache>(Encoding.UTF8.GetString(validationActorModel.ExternalDataCache));
            var fileDataCache = jsonSerializationService.Deserialize<FileDataCache>(Encoding.UTF8.GetString(validationActorModel.FileDataCache));
            var message = jsonSerializationService.Deserialize<Message>(new MemoryStream(validationActorModel.Message));

            var validationContext = new ValidationContext()
            {
                Input = message
            };

            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(validationContext).As<IValidationContext>();
                c.RegisterInstance(new Cache<IMessage>() { Item = message }).As<ICache<IMessage>>();
                c.RegisterInstance(internalDataCache).As<IInternalDataCache>();
                c.RegisterInstance(externalDataCache).As<IExternalDataCache>();
                c.RegisterInstance(fileDataCache).As<IFileDataCache>();
            }))
            {
                var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = validationActorModel.JobId;
                executionContext.TaskKey = _actorId.ToString();
                var logger = childLifeTimeScope.Resolve<ILogger>();
                try
                {
                    logger.LogDebug($"Validation Actor {executionContext.TaskKey} started learners:{validationContext.Input.Learners.Count}");
                    var preValidationOrchestrationService = childLifeTimeScope
                        .Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                    var errors = preValidationOrchestrationService.Execute();
                    logger.LogDebug($"actor {executionContext.TaskKey} validation done");

                    var errorString = jsonSerializationService.Serialize(errors);
                    logger.LogDebug("Actor completed job");
                    return Task.Run(() => errorString);
                }
                catch (Exception ex)
                {
                    ActorEventSource.Current.ActorMessage(this, "Exception-{0}", ex.ToString());
                    logger.LogError("Error while processing Actor job", ex);
                    throw;
                }
            }
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
    }
}
