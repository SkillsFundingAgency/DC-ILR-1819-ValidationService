using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Queueing;

namespace ESFA.DC.ILR.ValidationService.Stateless.Configuration
{
    public class ServiceBusTopicConfiguration : TopicConfiguration
    {
        public ServiceBusTopicConfiguration(string connectionString, string topicName, string subscriptionName, int maxConcurrentCalls, int minimumBackoffSeconds = 5, int maximumBackoffSeconds = 50, int maximumRetryCount = 10)
            : base(connectionString, topicName, subscriptionName, maxConcurrentCalls, minimumBackoffSeconds, maximumBackoffSeconds, maximumRetryCount)
        {
        }
    }
}
