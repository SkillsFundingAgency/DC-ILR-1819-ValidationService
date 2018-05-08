using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.JobContext
{
    public class ValidationContext : IValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }

    }
}
