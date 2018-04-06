using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.InternalData.Interface
{
    public interface IInternalDataCache
    {
        IReadOnlyCollection<int> AimTypes { get; }

        IReadOnlyCollection<int> CompStatuses { get; }
    }
}
