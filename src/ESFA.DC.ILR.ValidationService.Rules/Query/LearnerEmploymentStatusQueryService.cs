using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerEmploymentStatusQueryService : ILearnerEmploymentStatusQueryService
    {
        public int EmpStatForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateValue)
        {
            return learnerEmploymentStatuses?
                .Where(les => dateValue >= les.DateEmpStatApp)
                .OrderByDescending(les => les.DateEmpStatApp)
                .FirstOrDefault()?.EmpStat ?? 0;
        }

        public bool EmpStatsNotExistBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return !learnerEmploymentStatuses.Any(les => les.DateEmpStatApp < dateLearnStartDate);
        }

        public bool EmpStatsNotExistOnOrBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return !learnerEmploymentStatuses.Any(les => les.DateEmpStatApp <= dateLearnStartDate);
        }
    }
}
