using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerDPQueryService : ILearnerDPQueryService
    {
        public bool HasULNForLearnRefNumber(string learnRefNumber, long uln, ILearnerDestinationAndProgression learnerDestinationAndProgression)
        {
            if (learnerDestinationAndProgression == null)
            {
                return false;
            }

            return learnerDestinationAndProgression.LearnRefNumber == learnRefNumber
                && learnerDestinationAndProgression.ULN == uln;
        }

        public IDictionary<DateTime, IEnumerable<string>> OutTypesForStartDateAndTypes(IEnumerable<IDPOutcome> dpOutcomes, IEnumerable<string> outTypes)
        {
            if (dpOutcomes != null && outTypes != null)
            {
                if (dpOutcomes.Any(dp => outTypes.Contains(dp.OutType)))
                {
                    return dpOutcomes
                    .GroupBy(g => g.OutStartDate)
                    .ToDictionary(k => k.Key, v => v.Select(o => o.OutType));
                }
            }

            return null;
        }
    }
}
