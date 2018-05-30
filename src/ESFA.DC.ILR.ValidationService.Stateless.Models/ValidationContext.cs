using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class ValidationContext : IValidationContext
    {
        public IMessage Input { get; set; }
    }
}