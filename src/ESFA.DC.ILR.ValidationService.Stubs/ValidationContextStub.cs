using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ValidationContextStub : IPreValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }
        public string JobId { get; set; }
    }
}
