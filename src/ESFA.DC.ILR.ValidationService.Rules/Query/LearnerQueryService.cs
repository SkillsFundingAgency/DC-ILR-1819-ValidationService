using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerQueryService : ILearnerQueryService
    {
        public bool HasLearningDeliveryFAMCodeForType(ILearner learner, string famType, string famCode)
        {
            if (learner.LearningDeliveries == null)
            {
                return false;
            }

            return learner
                .LearningDeliveries
                .Where(ld => ld.LearningDeliveryFAMs != null)
                .SelectMany(ld => ld.LearningDeliveryFAMs)
                .Any(fam => fam.LearnDelFAMType == famType && fam.LearnDelFAMCode == famCode);
        }
    }
}
