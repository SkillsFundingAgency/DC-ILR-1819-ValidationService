using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Queueing;

namespace ESFA.DC.ILR.ValidationService.Stateless.Configuration
{
    public class ServiceBusQueueConfig : QueueConfiguration
    {
        public ServiceBusQueueConfig(string connectionString, string queueName, int maxConcurrentCalls, string topicName = null, int minimumBackoffSeconds = 5, int maximumBackoffSeconds = 50, int maximumRetryCount = 10)
            : base(connectionString, queueName, maxConcurrentCalls, topicName, minimumBackoffSeconds, maximumBackoffSeconds, maximumRetryCount)
        {
        }
    }
}
