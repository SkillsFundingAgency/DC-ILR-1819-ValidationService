using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class LearnerProviderServiceStub : IValidationItemProviderService<IEnumerable<ILearner>>
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerProviderServiceStub(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public IEnumerable<ILearner> Provide()
        {
            var message = _messageCache.Item;

            var learners = message.Learners.ToList();

            return learners;
        }
    }
}
