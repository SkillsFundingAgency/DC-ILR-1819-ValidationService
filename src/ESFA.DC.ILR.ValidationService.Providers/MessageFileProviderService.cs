using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageFileProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IMessageStringProviderService _stringProvider;
        private readonly ICache<string> _messageCache;

        public MessageFileProviderService(
            IXmlSerializationService xmlSerializationService,
            IMessageStringProviderService stringProvider,
            ICache<string> messageCache)
        {
            _xmlSerializationService = xmlSerializationService;
            _stringProvider = stringProvider;
            _messageCache = messageCache;
        }

        public IMessage Provide()
        {
            var fileContentCache = (Cache<string>)_messageCache;

            var fileContent = _stringProvider.Provide();
            fileContentCache.Item = fileContent;

            return string.IsNullOrEmpty(fileContent) ? null : _xmlSerializationService.Deserialize<Message>(fileContent);
        }
    }
}
