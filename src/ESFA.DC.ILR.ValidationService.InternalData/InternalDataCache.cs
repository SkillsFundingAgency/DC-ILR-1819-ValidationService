using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData
{
    public class InternalDataCache : IInternalDataCache
    {
        public IReadOnlyCollection<int> AimTypes { get; set; }
    }
}
