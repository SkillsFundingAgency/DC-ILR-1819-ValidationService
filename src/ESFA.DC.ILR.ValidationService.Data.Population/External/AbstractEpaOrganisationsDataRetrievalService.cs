using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public abstract class AbstractEpaOrganisationsDataRetrievalService : AbstractDataRetrievalService
    {
        protected AbstractEpaOrganisationsDataRetrievalService(ICache<IMessage> messageCache)
            : base(messageCache)
        {
        }

        public virtual IEnumerable<string> UniqueEpaOrgIdsFromMessage(IMessage message)
        {
            return message?.Learners?.SelectMany(l => l.LearningDeliveries.Select(ld => ld.EPAOrgID)).ToList() ?? new List<string>();
        }
    }
}
