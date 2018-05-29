using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationContext
    {
        IMessage Input { get; set; }
    }
}
