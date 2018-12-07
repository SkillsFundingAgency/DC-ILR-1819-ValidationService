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

        public bool HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes)
        {
            return employmentStatusMonitorings != null
                   && employmentStatusMonitorings
                       .Where(esm => esmTypes.Contains(esm.ESMType))
                       .GroupBy(x => x.ESMType)
                       .Any(g => g.Count() > 1);
        }

        public IEnumerable<string> GetDuplicatedEmploymentStatusMonitoringTypesForTypes(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes)
        {
            if (employmentStatusMonitorings == null || esmTypes == null)
            {
                return null;
            }

            return employmentStatusMonitorings
                .Where(esm => esmTypes.Contains(esm.ESMType))
                .GroupBy(x => x.ESMType)
                .Where(g => g.Count() > 1)
                .Select(s => s.Key);
        }
    }
}
