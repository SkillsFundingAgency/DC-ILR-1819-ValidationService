using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal
{
    public class InternalDataCache : IInternalDataCache
    {
        public IReadOnlyCollection<int> AimTypes { get; set; }

        public IReadOnlyCollection<int> CompStatuses { get; set; }
    }
}
