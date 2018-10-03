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
    }
}
