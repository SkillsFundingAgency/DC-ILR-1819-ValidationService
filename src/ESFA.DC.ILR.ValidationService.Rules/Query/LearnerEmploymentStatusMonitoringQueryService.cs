using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerEmploymentStatusMonitoringQueryService : ILearnerEmploymentStatusMonitoringQueryService
    {
        public bool HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, string esmType, int esmCode)
        {
            return learnerEmploymentStatuses != null
                && learnerEmploymentStatuses
                .Select(esm => esm.EmploymentStatusMonitorings
                .Any(esmt => esmt.ESMType == esmType && esmt.ESMCode == esmCode)).SingleOrDefault();
        }
    }
}
