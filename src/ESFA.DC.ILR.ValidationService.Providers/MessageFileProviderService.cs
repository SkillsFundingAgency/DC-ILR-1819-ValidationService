using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageFileProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IMessageStringProviderService _stringProvider;

        public MessageFileProviderService(IXmlSerializationService xmlSerializationService, IMessageStringProviderService stringProvider)
        {
            _xmlSerializationService = xmlSerializationService;
            _stringProvider = stringProvider;
        }

        public IMessage Provide()
        {
            var fileContent = _stringProvider.Provide();
            return string.IsNullOrEmpty(fileContent) ? null : _xmlSerializationService.Deserialize<Message>(fileContent);
        }
    }
}
