using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public abstract class AbstractLARSDataRetrievalService : AbstractDataRetrievalService
    {
        protected AbstractLARSDataRetrievalService(ICache<IMessage> messageCache)
            : base(messageCache)
        {
        }

        public IEnumerable<string> UniqueLearnAimRefsFromMessage(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Where(ld => ld.LearnAimRef != null)
                       .Select(ld => ld.LearnAimRef)
                       .Distinct()
                   ?? new List<string>();
        }
    }
}
