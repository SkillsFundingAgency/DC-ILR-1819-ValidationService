using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryAppFinRecordQueryService
    {
        bool HasAnyLearningDeliveryAFinCodesForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, IEnumerable<int> aFinCodes);
    }
}
