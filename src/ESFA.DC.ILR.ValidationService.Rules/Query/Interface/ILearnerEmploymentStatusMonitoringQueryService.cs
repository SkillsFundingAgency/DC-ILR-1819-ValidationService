using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusMonitoringQueryService
    {
        bool HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, string esmType, int esmCode);
    }
}
