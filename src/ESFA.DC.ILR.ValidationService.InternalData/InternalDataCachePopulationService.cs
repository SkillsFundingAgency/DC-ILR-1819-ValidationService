using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData
{
    public class InternalDataCachePopulationService : IInternalDataCachePopulationService
    {
        private readonly IInternalDataCache _internalDataCache;

        public InternalDataCachePopulationService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public void Populate()
        {
            var internalDataCache = (InternalDataCache)_internalDataCache;

            internalDataCache.AimTypes = BuildAimTypes();
        }

        public IReadOnlyCollection<int> BuildAimTypes()
        {
            return new HashSet<int>()
            {
                1,
                3,
                4,
                5,
            };
        }
    }
}
