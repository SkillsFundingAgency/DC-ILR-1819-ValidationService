using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerFAMQueryService : ILearnerFAMQueryService
    {
        public bool HasAnyLearnerFAMCodesForType(IEnumerable<ILearnerFAM> learnerFams, string famType, IEnumerable<int> famCodes)
        {
            if (learnerFams == null || famCodes == null)
            {
                return false;
            }

            return learnerFams.Any(lfam => lfam.LearnFAMType.CaseInsensitiveEquals(famType) && famCodes.Contains(lfam.LearnFAMCode));
        }

        public bool HasLearnerFAMCodeForType(IEnumerable<ILearnerFAM> learnerFams, string famType, int famCode)
        {
            return learnerFams != null && learnerFams.Any(ldfam => ldfam.LearnFAMType.CaseInsensitiveEquals(famType) && ldfam.LearnFAMCode == famCode);
        }

        public bool HasLearnerFAMType(IEnumerable<ILearnerFAM> learnerFams, string famType)
        {
            return learnerFams != null && learnerFams.Any(ldfam => ldfam.LearnFAMType.CaseInsensitiveEquals(famType));
        }

        public bool HasAnyLearnerFAMTypes(IEnumerable<ILearnerFAM> learnerFams, IEnumerable<string> famTypes)
        {
            if (learnerFams == null || famTypes == null)
            {
                return false;
            }

            return learnerFams.Any(ldfam => famTypes.Any(x => x.CaseInsensitiveEquals(ldfam.LearnFAMType)));
        }

        public int GetLearnerFAMsCountByFAMType(IEnumerable<ILearnerFAM> learnerFaMs, string famType)
        {
            return learnerFaMs?.Count(d => d.LearnFAMType.CaseInsensitiveEquals(famType)) ?? 0;
        }
    }
}
