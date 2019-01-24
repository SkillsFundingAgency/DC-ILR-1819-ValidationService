﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryAppFinRecordQueryService
    {
        bool HasAnyLearningDeliveryAFinCodesForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, IEnumerable<int> aFinCodes);

        bool HasAnyLearningDeliveryAFinCodeForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, int? aFinCode);

        bool HasAnyLearningDeliveryAFinCodes(IEnumerable<IAppFinRecord> appFinRecords, IEnumerable<int> aFinCodes);

        IAppFinRecord GetLatestAppFinRecord(IReadOnlyCollection<IAppFinRecord> appFinRecords, string appFinType, int appFinCode);
    }
}
