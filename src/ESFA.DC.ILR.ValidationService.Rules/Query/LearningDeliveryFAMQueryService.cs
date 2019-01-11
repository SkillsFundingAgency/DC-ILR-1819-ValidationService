﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryFAMQueryService : ILearningDeliveryFAMQueryService
    {
        public bool HasAnyLearningDeliveryFAMCodesForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, IEnumerable<string> famCodes)
        {
            return GetLearningDeliveryFAMsForTypeAndCodes(learningDeliveryFAMs, famType, famCodes)?
                       .Any()
                   ?? false;
        }

        public bool HasLearningDeliveryFAMCodeForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, string famCode)
        {
            return GetLearningDeliveryFAMsForTypeAndCode(learningDeliveryFAMs, famType, famCode)?
                       .Any()
                   ?? false;
        }

        public bool HasLearningDeliveryFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType)
        {
            return GetLearningDeliveryFAMsForType(learningDeliveryFAMs, famType)?.
                       Any()
                   ?? false;
        }

        public bool HasLearningDeliveryFAMTypeForDate(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, DateTime date)
        {
            return GetLearningDeliveryFAMsForType(learningDeliveryFAMs, famType)?.Any(ldfam => ldfam.LearnDelFAMDateFromNullable == date) ?? false;
        }

        public int GetLearningDeliveryFAMsCountByFAMType(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs, string famType)
        {
            return GetLearningDeliveryFAMsForType(learningDeliveryFAMs, famType)?.Count() ?? 0;
        }

        public IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType)
        {
            return learningDeliveryFams?.Where(fam => HasFamType(fam, famType));
        }

        public IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsForTypeAndCode(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType, string famCode)
        {
            return learningDeliveryFams?.Where(fam => HasFamType(fam, famType) && HasFamCode(fam, famCode));
        }

        public IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsForTypeAndCodes(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType, IEnumerable<string> famCodes)
        {
            if (famCodes == null)
            {
                return null;
            }

            return learningDeliveryFams?.Where(fam => HasFamType(fam, famType) && famCodes.Contains(fam.LearnDelFAMCode));
        }

        public bool HasFamType(ILearningDeliveryFAM learningDeliveryFam, string famType)
        {
            return learningDeliveryFam.LearnDelFAMType.CaseInsensitiveEquals(famType);
        }

        public bool HasFamCode(ILearningDeliveryFAM learningDeliveryFam, string famCode)
        {
            return learningDeliveryFam.LearnDelFAMCode == famCode;
        }
    }
}
