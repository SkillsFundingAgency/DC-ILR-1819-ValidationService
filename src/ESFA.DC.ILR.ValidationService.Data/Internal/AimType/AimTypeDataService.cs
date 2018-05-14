using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AimType.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AimType
{
    public class AimTypeDataService : IAimTypeDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public AimTypeDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int aimType)
        {
            return _internalDataCache.AimTypes.Contains(aimType);
        }
    }
}
