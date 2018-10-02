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

        public async Task<bool> Handle(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            using (var childLifeTimeScope = _parentLifeTimeScope
                .BeginLifetimeScope(c =>
                {
                    var fileName = jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString();
                    if (fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        c.RegisterType<AzureStorageCompressedFileContentStringProviderService>()
                            .As<IMessageStreamProviderService>();
                    }
                    else
                    {
                        c.RegisterType<AzureStorageFileContentStringProviderService>()
                            .As<IMessageStreamProviderService>();
                    }

                    c.RegisterInstance(
                        new PreValidationContext
                        {
                            JobId = jobContextMessage.JobId.ToString(),
                            Input = fileName,
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

                    logger.LogDebug("inside process message validate");

                    var preValidationOrchestrationService = childLifeTimeScope
                        .Resolve<IPreValidationOrchestrationService<IValidationError>>();

                    var validationContext = childLifeTimeScope.Resolve<IPreValidationContext>();

                    await preValidationOrchestrationService.ExecuteAsync(validationContext, cancellationToken);

                    // Update the file name, as it could have been a zip which we have extracted now so needs updating in the message
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = validationContext.Input;

                    // populate the keys into jobcontextmessage
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount] =
                        validationContext.InvalidLearnRefNumbersCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount] =
                        validationContext.ValidLearnRefNumbersCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalErrorCount] =
                        validationContext.ValidationTotalErrorCount;
                    jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalWarningCount] =
                        validationContext.ValidationTotalWarningCount;

                    logger.LogDebug("Validation complete");
                    ServiceEventSource.Current.ServiceMessage(_context, "Validation complete");
                    return true;
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
