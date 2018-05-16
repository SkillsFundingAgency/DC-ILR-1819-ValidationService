using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Management.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Castle.Components.DictionaryAdapter.Xml;
using DC.JobContextManager;
using DC.JobContextManager.Interface;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Handlers;
using ESFA.DC.ILR.ValidationService.Stateless.Listeners;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
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
        private ILogger _logger;

        public Stateless(
            StatelessServiceContext context,
            ILifetimeScope parentLifeTimeScope,
            ServiceBusOptions seviceBusOptions)
            : base(context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _logger = parentLifeTimeScope.Resolve<ILogger>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
//            var jobContextManager = new JobContextManager<JobContextMessage>(
//                _parentLifeTimeScope.Resolve<IQueueSubscriptionService<JobContextMessage>>(),
//                _parentLifeTimeScope.Resolve<IQueuePublishService<JobContextMessage>>(),
//                _parentLifeTimeScope.Resolve<IAuditor>(),
//                _parentLifeTimeScope.Resolve<IMapper<JobContextMessage, JobContextMessage>>(),
//                _parentLifeTimeScope.Resolve<MessageHandler>().Handle1,
//                _logger);

            yield return new ServiceInstanceListener(
                context => _parentLifeTimeScope.Resolve<IJobContextManager>(),
                "ValidationService-SBQueueListener");
        }
    }
}
