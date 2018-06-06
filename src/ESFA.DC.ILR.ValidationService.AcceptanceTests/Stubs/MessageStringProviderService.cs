using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageStringProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IPreValidationContext _validationContext;

        private IMessage _message;

        public MessageStringProviderService(IXmlSerializationService xmlSerializationService, IPreValidationContext validationContext)
        {
            _xmlSerializationService = xmlSerializationService;
            _validationContext = validationContext;
        }

        public IMessage Provide()
        {
            return _message ?? (_message = _xmlSerializationService.Deserialize<Message>(_validationContext.Input));
        }
    }
}
