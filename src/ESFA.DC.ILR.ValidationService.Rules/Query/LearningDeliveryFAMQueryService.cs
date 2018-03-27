using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryFAMQueryService : ILearningDeliveryFAMQueryService
    {
        public bool HasAnyLearningDeliveryFAMCodesForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, IEnumerable<string> famCodes)
        {
            if (learningDeliveryFAMs == null || famCodes == null)
            {
                return false;
            }

            return learningDeliveryFAMs.Any(ldfam => ldfam.LearnDelFAMType == famType && famCodes.Contains(ldfam.LearnDelFAMCode));
        }

        public bool HasLearningDeliveryFAMCodeForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, string famCode)
        {
            if (learningDeliveryFAMs == null)
            {
                return false;
            }

            return learningDeliveryFAMs.Any(ldfam => ldfam.LearnDelFAMType == famType && ldfam.LearnDelFAMCode == famCode);
        }

        public bool HasLearningDeliveryFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType)
        {
            if (learningDeliveryFAMs == null)
            {
                return false;
            }

            return learningDeliveryFAMs.Any(ldfam => ldfam.LearnDelFAMType == famType);
        }
    }
}
