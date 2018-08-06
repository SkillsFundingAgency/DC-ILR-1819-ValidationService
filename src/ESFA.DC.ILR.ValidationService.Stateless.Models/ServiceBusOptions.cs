namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ServiceBusOptions
    {
        public string JobsQueueName { get; set; }

        public string AuditQueueName { get; set; }

        public string JobStatusQueueName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string FundingCalcSubscriptionName { get; set; }
    }
}
