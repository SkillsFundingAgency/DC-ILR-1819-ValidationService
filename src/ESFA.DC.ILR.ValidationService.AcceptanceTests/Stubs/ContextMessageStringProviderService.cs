using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ContextMessageStringProviderService : IMessageStringProviderService
    {
        private readonly IPreValidationContext _validationContext;

        public ContextMessageStringProviderService(IPreValidationContext validationContext)
        {
            _validationContext = validationContext;
        }

        public string Provide()
        {
            return _validationContext.Input;
        }
    }
}
