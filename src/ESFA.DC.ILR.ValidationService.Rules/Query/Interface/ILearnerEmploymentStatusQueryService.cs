using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService
    {
        /// <summary>
        /// Returns all employment statuses where they could apply
        /// </summary>
        /// <param name="learnerEmploymentStatuses"></param>
        /// <param name="dateEmpStatApp"></param>
        /// <returns></returns>
        // IEnumerable<int> EmpStatsForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateEmpStatApp);

        /// <summary>
        /// returns the latest employment status that applies
        /// </summary>
        /// <param name="learnerEmploymentStatuses"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        int EmpStatForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateValue);

        bool EmpStatsNotExistBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate);

        bool EmpStatsNotExistOnOrBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate);
    }
}
