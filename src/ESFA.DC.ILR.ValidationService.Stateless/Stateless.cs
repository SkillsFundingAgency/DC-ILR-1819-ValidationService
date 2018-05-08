using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.JobContext;
using ESFA.DC.ILR.ValidationService.Stateless.Listeners;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.ILR.ValidationService.Stateless.Modules;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace ESFA.DC.ILR.ValidationService.Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Stateless : StatelessService
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly string _queueName;
        private readonly string _serviceBusConnectionString;
        private ILogger _logger;

        public Stateless(StatelessServiceContext context, ILifetimeScope parentLifeTimeScope,
            ServiceBusOptions seviceBusOptions)
            : base(context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;

            //get config values
            _queueName = seviceBusOptions.QueueName;
            _serviceBusConnectionString = seviceBusOptions.ServiceBusConnectionString;
            _logger = parentLifeTimeScope.Resolve<ILogger>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            yield return new ServiceInstanceListener(
                context => new ServiceBusQueueListener(ProcessMessageHandler,
                    _serviceBusConnectionString, _queueName, _logger),
                "StatelessService-ServiceBusQueueListener");

        }

        async Task ProcessMessageHandler(ServiceBusQueueListenerModel listernerModel)
        {
            var jobContext = JsonConvert.DeserializeObject<IJobContextMessage>(Encoding.UTF8.GetString(listernerModel.Message.Body));

            var validationContext = new ValidationContext()
            {
                Input = "",
                Output = ""
            };


            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope(c => c.RegisterInstance(validationContext).As<IValidationContext>()))
            {
                var logger = childLifeTimeScope.Resolve<ILogger>();

                try
                {
                    logger.LogInfo("inside processmessage validate");
                    var ruleSetOrchestrationService = childLifeTimeScope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                    var errors = ruleSetOrchestrationService.Execute(validationContext);

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Job done");

                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Exception-{0}", ex.ToString());
                    logger.LogError("Error while processing job", ex);
                    throw;
                }
            }
        }
    }
}
