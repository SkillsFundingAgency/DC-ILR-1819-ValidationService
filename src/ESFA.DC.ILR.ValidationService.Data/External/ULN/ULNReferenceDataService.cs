using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.ULN
{
    public class ULNReferenceDataService : IULNReferenceDataService
    {
        private readonly IExternalDataCache _referenceDataCache;

        public ULNReferenceDataService(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        public bool Exists(long? uln)
        {
            return uln.HasValue &&
                _referenceDataCache.ULNs.Contains(uln.Value);
        }
    }
}
