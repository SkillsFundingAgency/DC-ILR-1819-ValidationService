﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerFAMQueryService
    {
        bool HasAnyLearnerFAMCodesForType(IEnumerable<ILearnerFAM> learnerFAMs, string famType, IEnumerable<long> famCodes);

        bool HasLearnerFAMCodeForType(IEnumerable<ILearnerFAM> learnerFAMs, string famType, long famCode);

        bool HasLearnerFAMType(IEnumerable<ILearnerFAM> learnerFAMs, string famType);

        bool HasAnyLearnerFAMTypes(IEnumerable<ILearnerFAM> learnerFams, IEnumerable<string> famTypes);
    }
}
