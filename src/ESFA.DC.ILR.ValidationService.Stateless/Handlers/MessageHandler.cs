using System.Fabric;

namespace ESFA.DC.ILR.ValidationService.Stateless.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Stateless.Models;
    using ESFA.DC.JobContext;
    using ESFA.DC.JobContext.Interface;
    using ESFA.DC.Logging.Interfaces;

    public class MessageHandler : IMessageHandler
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly StatelessServiceContext _context;

        public MessageHandler(ILifetimeScope parentLifeTimeScope, StatelessServiceContext context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = context;
        }

        public Task<bool> Handle(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            var validationContext = new PreValidationContext()
            {
                JobId = jobContextMessage.JobId.ToString(),
                Input = jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString()
            };

            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope(c => c.RegisterInstance(validationContext).As<IPreValidationContext>()))
            {
                var executionContext = (Logging.ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = jobContextMessage.JobId.ToString();
                var logger = childLifeTimeScope.Resolve<ILogger>();

                try
                {
                    var azureStorageModel = childLifeTimeScope.Resolve<AzureStorageModel>();
                    azureStorageModel.AzureContainerReference =
                        jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString();

                    logger.LogInfo("inside processmessage validate");

                    var preValidationOrchestrationService = childLifeTimeScope
                        .Resolve<IPreValidationOrchestrationService<ILearner, IValidationError>>();

                    var errors = preValidationOrchestrationService.Execute(validationContext);

                    // TODO: do something with the errors

                    logger.LogInfo("Job complete");
                    ServiceEventSource.Current.ServiceMessage(_context, "Job complete");
                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());
                    logger.LogError("Error while processing job", ex);
                    throw;
                }
            }
        }
    }
}
