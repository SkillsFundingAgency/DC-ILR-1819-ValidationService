using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class AzureCosmosOptions
    {
        public string CosmosEndpointUrl { get; set; }

        public string CosmosAuthKeyOrResourceToken { get; set; }
    }
}
