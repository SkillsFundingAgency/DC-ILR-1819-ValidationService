using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public abstract class AbstractOrganisationsDataRetrievalService : AbstractDataRetrievalService
    {
        protected AbstractOrganisationsDataRetrievalService(ICache<IMessage> messageCache)
            : base(messageCache)
        {
        }

        public IEnumerable<long> UniqueUKPRNsFromMessage(IMessage message)
        {
            return UniqueLearningProviderUKPRNFromMessage(message)
                .Union(UniqueLearnerPrevUKPRNsFromMessage(message))
                .Union(UniqueLearnerPMUKPRNsFromMessage(message))
                .Union(UniqueLearningDeliveryPartnerUKPRNsFromMessage(message))
                .Distinct();
        }

        public virtual IEnumerable<long> UniqueLearningProviderUKPRNFromMessage(IMessage message)
        {
            return
                new List<long>
                {
                    message.LearningProviderEntity.UKPRN
                };
        }

        public virtual IEnumerable<long> UniqueLearnerPrevUKPRNsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Where(l => l.PrevUKPRNNullable != null)
                       .Select(l => (long)l.PrevUKPRNNullable)
                       .Distinct()
                   ?? new List<long>();
        }

        public virtual IEnumerable<long> UniqueLearnerPMUKPRNsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Where(l => l.PMUKPRNNullable != null)
                       .Select(l => (long)l.PMUKPRNNullable)
                       .Distinct()
                   ?? new List<long>();
        }

        public virtual IEnumerable<long> UniqueLearningDeliveryPartnerUKPRNsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Where(ld => ld.PartnerUKPRNNullable != null)
                       .Select(ld => (long)ld.PartnerUKPRNNullable)
                       .Distinct()
                   ?? new List<long>();
        }
    }
}
