using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.ULN
{
    public class ULNReferenceDataService : IULNReferenceDataService
    {
        private readonly IReferenceDataCache _referenceDataCache;

        public ULNReferenceDataService(IReferenceDataCache referenceDataCache)
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
