using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class InternalDataCacheStub : IInternalDataCache
    {
        public InternalDataCacheStub()
        {
            AimTypes = new HashSet<int>();
        }

        public IReadOnlyCollection<int> AimTypes { get; }
    }
}
