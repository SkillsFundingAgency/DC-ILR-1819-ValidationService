using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace ESFA.DC.ILR.ValidationService.Stateless.Listeners
{
    public class ServiceBusQueueListener : ICommunicationListener
    {

        public ServiceBusQueueListener(Func<ServiceBusQueueListernerModel, Task> callback, string serviceBusConnectionString, string queueName, ILogger logger)
        {
            
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
