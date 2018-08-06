using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3
{
    public class QUALENT3DataService : IQUALENT3DataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public QUALENT3DataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(string qualent3)
        {
            return _internalDataCache.QUALENT3s.Contains(qualent3);
        }
    }
}
