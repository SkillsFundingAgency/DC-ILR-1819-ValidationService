using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IInternalDataCache
    {
        IAcademicYear AcademicYear { get; }

        IReadOnlyCollection<int> AimTypes { get; }

        IReadOnlyCollection<int> CompStatuses { get; }

        IReadOnlyCollection<int> EmpOutcomes { get; }

        IReadOnlyCollection<int> FundModels { get; }
    }
}
