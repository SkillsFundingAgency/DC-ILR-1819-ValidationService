using ESFA.DC.ILR.ValidationService.ExternalData.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.ULN.Interface;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.ExternalData.ULN
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
