using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class InternalDataCache : IInternalDataCache
    {
        public IAcademicYear AcademicYear { get; set; }

        public IReadOnlyCollection<int> AimTypes { get; set; }

        public IReadOnlyCollection<int> CompStatuses { get; set; }

        public IReadOnlyCollection<int> EmpOutcomes { get; set; }

        public IReadOnlyCollection<int> FundModels { get; set; }
    }
}
