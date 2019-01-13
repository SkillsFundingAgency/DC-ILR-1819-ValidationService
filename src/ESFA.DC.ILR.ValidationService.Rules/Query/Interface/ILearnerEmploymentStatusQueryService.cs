using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService
    {
        /// <summary>
        /// returns the latest Learner Employment Status that applies for a given Date
        /// </summary>
        /// <param name="learnerEmploymentStatuses"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        ILearnerEmploymentStatus LearnerEmploymentStatusForDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime datetime);

        bool EmpStatsNotExistBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        bool EmpStatsNotExistOnOrBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);
    }
}
