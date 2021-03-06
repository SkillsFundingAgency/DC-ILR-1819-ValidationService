﻿using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ValidationService.Stateless.Handlers
{
    public class MessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly StatelessServiceContext _context;

        public MessageHandler(ILifetimeScope parentLifeTimeScope, StatelessServiceContext context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _context = context;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            using (var childLifeTimeScope = _parentLifeTimeScope
                .BeginLifetimeScope(c =>
                {
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
                                .KeyValuePairs[JobContextMessageKey.ValidationErrorLookups].ToString(),
                            Tasks = jobContextMessage.Topics[jobContextMessage.TopicPointer].Tasks.SelectMany(x => x.Tasks)
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
                catch (OutOfMemoryException oom)
                {
                    Environment.FailFast("Validation Service Out Of Memory", oom);
                    throw;
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
