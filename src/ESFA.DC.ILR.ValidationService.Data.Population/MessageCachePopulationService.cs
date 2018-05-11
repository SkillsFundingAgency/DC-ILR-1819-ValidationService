using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class MessageCachePopulationService : IMessageCachePopulationService
    {
        private readonly ICache<IMessage> _messageCache;
        private readonly IValidationItemProviderService<IMessage> _messageValidationItemProviderService;

        public MessageCachePopulationService(ICache<IMessage> messageCache, IValidationItemProviderService<IMessage> messageValidationItemProviderService)
        {
            _messageCache = messageCache;
            _messageValidationItemProviderService = messageValidationItemProviderService;
        }

        public void Populate()
        {
            var messageCache = (Cache<IMessage>)_messageCache;
            messageCache.Item = _messageValidationItemProviderService.Provide();
        }

        public void Populate(IMessage data)
        {
            var messageCache = (Cache<IMessage>)_messageCache;
            messageCache.Item = data;
        }
    }
}
