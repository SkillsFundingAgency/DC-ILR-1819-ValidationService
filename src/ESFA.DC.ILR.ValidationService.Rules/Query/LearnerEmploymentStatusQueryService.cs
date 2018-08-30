using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public class LearnerEmploymentStatusQueryService : ILearnerEmploymentStatusQueryService
    {
        public int? EmpStatForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateEmpStatApp)
        {
            if (learnerEmploymentStatuses == null)
            {
                return null;
            }

            return learnerEmploymentStatuses.Where(les => les.DateEmpStatApp == dateEmpStatApp).Select(les => les.EmpStat).FirstOrDefault();
        }

        public IEnumerable<int> EmpStatsForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateEmpStatApp)
        {
            if (learnerEmploymentStatuses == null)
            {
                return null;
            }

            return learnerEmploymentStatuses.Where(les => les.DateEmpStatApp >= dateEmpStatApp).Select(les => les.EmpStat).ToList();
        }

        public bool EmpStatsNotExistBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate)
        {
            if (learnerEmploymentStatuses == null)
            {
                return true;
            }

            return learnerEmploymentStatuses.Where(les => les.DateEmpStatApp < dateLearnStartDate).Count() == 0;
        }
    }
}
