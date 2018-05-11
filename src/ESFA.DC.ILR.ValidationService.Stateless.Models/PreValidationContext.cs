using System.Runtime.Remoting.Messaging;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class PreValidationContext : IPreValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }
        public string JobId { get; set; }
    }
}
