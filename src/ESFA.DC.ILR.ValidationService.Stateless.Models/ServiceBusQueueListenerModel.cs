using System.Threading;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ServiceBusQueueListenerModel
    {
        public Microsoft.Azure.ServiceBus.Message Message { get; set; }

        public CancellationToken Token { get; set; }
    }
}
