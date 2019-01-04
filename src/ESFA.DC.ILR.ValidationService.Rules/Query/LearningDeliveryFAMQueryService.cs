using System;
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
            return learningDeliveryFAMs != null && learningDeliveryFAMs.Any(ldfam => ldfam.LearnDelFAMType == famType && ldfam.LearnDelFAMCode == famCode);
        }

        public bool HasLearningDeliveryFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType)
        {
            return learningDeliveryFAMs != null && learningDeliveryFAMs.Any(ldfam => ldfam.LearnDelFAMType == famType);
        }

        public bool HasAnyLearningDeliveryFAMTypes(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, IEnumerable<string> famTypes)
        {
            return learningDeliveryFAMs != null
                   && famTypes != null
                   && learningDeliveryFAMs.Any(ldfam => famTypes.Contains(ldfam.LearnDelFAMType));
        }

        public bool HasLearningDeliveryFAMTypeForDate(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, DateTime date)
        {
            return learningDeliveryFAMs != null
                && famType != null
                && learningDeliveryFAMs
                .Any(ldfam => ldfam.LearnDelFAMType == famType
                    && ldfam.LearnDelFAMDateFromNullable == date);
        }

        public ILearningDeliveryFAM GetLearningDeliveryFAMByTypeAndLatestByDateFrom(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string learnDelFAMType)
        {
            return learningDeliveryFAMs?
                .Where(f => f.LearnDelFAMType == learnDelFAMType)
                .OrderByDescending(f => f.LearnDelFAMDateFromNullable)
                .FirstOrDefault();
        }

        public int GetLearningDeliveryFAMsCountByFAMType(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs, string famType)
            => learningDeliveryFAMs?.Where(d => d.LearnDelFAMType == famType).Count() ?? 0;
    }
}
