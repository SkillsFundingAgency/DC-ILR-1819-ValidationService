using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerEmploymentStatusQueryService : ILearnerEmploymentStatusQueryService
    {
        public ILearnerEmploymentStatus LearnerEmploymentStatusForDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date)
        {
            return learnerEmploymentStatuses?
                .Where(les => date >= les.DateEmpStatApp)
                .OrderByDescending(les => les.DateEmpStatApp)
                .FirstOrDefault();
        }

        public bool EmpStatsNotExistBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return !learnerEmploymentStatuses.Any(les => les.DateEmpStatApp < date);
        }

        public bool EmpStatsNotExistOnOrBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return !learnerEmploymentStatuses.Any(les => les.DateEmpStatApp <= date);
        }
    }
}
