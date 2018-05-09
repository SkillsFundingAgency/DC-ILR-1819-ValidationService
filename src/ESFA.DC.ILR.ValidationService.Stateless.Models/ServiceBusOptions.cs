namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ServiceBusOptions
    {
        public string QueueName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }
    }
}
