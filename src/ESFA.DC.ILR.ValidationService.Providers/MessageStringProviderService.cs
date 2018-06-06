using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageStringProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IStringProviderService _stringProvider;

        public MessageStringProviderService(IXmlSerializationService xmlSerializationService, IStringProviderService stringProvider)
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
