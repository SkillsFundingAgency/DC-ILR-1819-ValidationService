using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation
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
