using ESFA.DC.ILR.ValidationService.ExternalData.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.Organisation.Interface;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.ExternalData.Organisation
{
    public class OrganisationReferenceDataService : IOrganisationReferenceDataService
    {
        private readonly IReferenceDataCache _referenceDataCache;

        public OrganisationReferenceDataService(IReferenceDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        public bool UkprnExists(long ukprn)
        {
            return _referenceDataCache.UKPRNs.Contains(ukprn);
        }
    }
}
