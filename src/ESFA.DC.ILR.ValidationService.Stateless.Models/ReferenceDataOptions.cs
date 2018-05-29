using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ReferenceDataOptions
    {
        public string LARSConnectionString { get; set; }

        public string PostcodesConnectionString { get; set; }

        public string ULNConnectionstring { get; set; }
    }
}
