using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.KeyGenerator.Interface;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ValidationService.Stateless.Handlers
{
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
            var keyGenerator = _parentLifeTimeScope.Resolve<IKeyGenerator>();
            var ukprn = Convert.ToInt64(jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn]);

            // populate the keys into jobcontextmessage - this should be done in Orchestrator
            jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbers] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.ValidationInvalidLearners);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.ValidationValidLearners);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.ValidationErrors);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrorLookups] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.ValidationErrorsLookup);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.FundingAlbOutput);
            jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingFm35Output] = keyGenerator.GenerateKey(ukprn, jobContextMessage.JobId, TaskKeys.FundingFm35Output);

            using (var childLifeTimeScope = _parentLifeTimeScope
                .BeginLifetimeScope(c =>
                {
                    c.RegisterType<AzureStorageFileContentStringProviderService>().As<IMessageStringProviderService>();
                    c.RegisterInstance(
                        new PreValidationContext
                        {
                            JobId = jobContextMessage.JobId.ToString(),
                            Input = jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString(),
                            InvalidLearnRefNumbersKey = jobContextMessage
                                .KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbers].ToString(),
                            ValidLearnRefNumbersKey =
                                jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers].ToString(),
                            ValidationErrorsKey = jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors]
                                .ToString(),
                            ValidationErrorMessageLookupKey = jobContextMessage
                                .KeyValuePairs[JobContextMessageKey.ValidationErrorLookups].ToString()
                        }).As<IPreValidationContext>();
                }))
            {
                var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = jobContextMessage.JobId.ToString();
                var logger = childLifeTimeScope.Resolve<ILogger>();

                try
                {
                    var azureStorageModel = childLifeTimeScope.Resolve<AzureStorageModel>();
                    azureStorageModel.AzureContainerReference =
                        jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString();

                    logger.LogDebug("inside processmessage validate");

                    var preValidationOrchestrationService = childLifeTimeScope
                        .Resolve<IPreValidationOrchestrationService<IValidationError>>();

                    var validationContext = childLifeTimeScope.Resolve<IPreValidationContext>();

                    preValidationOrchestrationService.Execute(validationContext);

                    // populate the keys into jobcontextmessage
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount] = validationContext.InvalidLearnRefNumbersCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount] = validationContext.ValidLearnRefNumbersCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalErrorCount] = validationContext.ValidationTotalErrorCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalWarningCount] = validationContext.ValidationTotalWarningCount;

                    logger.LogDebug("Job complete");
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
