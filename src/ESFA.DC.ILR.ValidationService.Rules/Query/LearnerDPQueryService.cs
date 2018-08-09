using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerDPQueryService : ILearnerDPQueryService
    {
        public bool HasULNForLearnRefNumber(string learnRefNumber, long uln, IEnumerable<ILearnerDestinationAndProgression> learnerDestinationAndProgressions)
        {
            if (learnerDestinationAndProgressions == null)
            {
                return false;
            }

            return learnerDestinationAndProgressions.Any(dp => dp.LearnRefNumber == learnRefNumber && dp.ULN == uln);
        }
    }
}
