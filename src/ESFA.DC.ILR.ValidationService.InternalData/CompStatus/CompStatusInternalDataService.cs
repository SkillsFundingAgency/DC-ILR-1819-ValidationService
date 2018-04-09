using System.Linq;
using ESFA.DC.ILR.ValidationService.InternalData.CompStatus.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData.CompStatus
{
    public class CompStatusInternalDataService : ICompStatusInternalDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public CompStatusInternalDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int compStatus)
        {
            return _internalDataCache.CompStatuses.Contains(compStatus);
        }
    }
}
