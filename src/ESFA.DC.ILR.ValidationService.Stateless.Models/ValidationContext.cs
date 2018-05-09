using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ValidationContext : IValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }

    }
}
