using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus.Interface;
using IInternalDataCache = ESFA.DC.ILR.ValidationService.Data.Interface.IInternalDataCache;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus
{
    public class CompStatusDataService : ICompStatusDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public CompStatusDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int compStatus)
        {
            return _internalDataCache.CompStatuses.Contains(compStatus);
        }
    }
}
