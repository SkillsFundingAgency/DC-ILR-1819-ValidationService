using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IMessageStringProviderService _stringProvider;

        public MessageProviderService(IXmlSerializationService xmlSerializationService, IMessageStringProviderService stringProvider)
        {
            _xmlSerializationService = xmlSerializationService;
            _stringProvider = stringProvider;
        }

        public IMessage Provide()
        {
            return _xmlSerializationService.Deserialize<Message>(_stringProvider.Provide());
        }
    }
}
