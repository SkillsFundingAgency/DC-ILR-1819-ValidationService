using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ServiceBusOptions
    {
        public string QueueName { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }
    }
}
