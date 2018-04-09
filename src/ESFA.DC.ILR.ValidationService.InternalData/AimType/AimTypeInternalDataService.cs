using System.Linq;
using ESFA.DC.ILR.ValidationService.InternalData.AimType.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData.AimType
{
    public class AimTypeInternalDataService : IAimTypeInternalDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public AimTypeInternalDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int aimType)
        {
            return _internalDataCache.AimTypes.Contains(aimType);
        }
    }
}
