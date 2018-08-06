using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public abstract class AbstractDataRetrievalService
    {
        protected ICache<IMessage> _messageCache;

        protected AbstractDataRetrievalService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }
    }
}
