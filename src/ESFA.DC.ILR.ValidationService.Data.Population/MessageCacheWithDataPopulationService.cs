using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class MessageCacheWithDataPopulationService : IMessageCacheWithDataPopulationService
    {
        private readonly ICache<IMessage> _messageCache;

        public MessageCacheWithDataPopulationService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public void Populate(IMessage data)
        {
            var messageCache = (Cache<IMessage>)_messageCache;
            messageCache.Item = data;
        }
    }
}
