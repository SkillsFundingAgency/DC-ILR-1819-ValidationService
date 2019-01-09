using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerEmploymentStatusQueryService : ILearnerEmploymentStatusQueryService
    {
        public IEnumerable<int> EmpStatsForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateValue)
        {
            return learnerEmploymentStatuses?.Where(les => dateValue >= les.DateEmpStatApp).Select(les => les.EmpStat).ToList();
        }

        public bool EmpStatsNotExistBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return learnerEmploymentStatuses.Where(les => les.DateEmpStatApp < dateLearnStartDate).Count() == 0;
        }

        public bool EmpStatsNotExistOnOrBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return learnerEmploymentStatuses.Where(les => les.DateEmpStatApp <= dateLearnStartDate).Count() == 0;
        }
    }
}
