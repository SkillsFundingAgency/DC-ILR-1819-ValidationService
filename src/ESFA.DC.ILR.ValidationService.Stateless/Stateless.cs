using System.Collections.Generic;
using System.Fabric;
using Autofac;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.JobContext;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

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
            yield return new ServiceInstanceListener(
                context => _parentLifeTimeScope.Resolve<IJobContextManager<JobContextMessage>>(),
                "ValidationService-SBQueueListener");
        }
    }
}
