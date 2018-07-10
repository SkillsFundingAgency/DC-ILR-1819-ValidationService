using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryAppFinRecordQueryService : ILearningDeliveryAppFinRecordQueryService
    {
        public bool HasAnyLearningDeliveryAFinCodesForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, IEnumerable<int> aFinCodes)
        {
            if (appFinRecords == null || aFinCodes == null)
            {
                return false;
            }

            return appFinRecords.Any(afr => afr.AFinType == aFinType && aFinCodes.Contains(afr.AFinCode));
        }
    }
}
