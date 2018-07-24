using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService
    {
        int? EmpStatForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateEmpStatApp);
    }
}
