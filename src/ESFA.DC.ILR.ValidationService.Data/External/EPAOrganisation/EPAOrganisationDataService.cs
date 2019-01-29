using System;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation
{
    public class EPAOrganisationDataService : IEPAOrganisationDataService
    {
        private readonly IExternalDataCache _referenceDataCache;

        public EPAOrganisationDataService(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        public bool IsValidEpaOrg(string epaOrgId, int? stdCode, DateTime learnPlanEndDate)
        {
            if (epaOrgId == null || !stdCode.HasValue)
            {
                return false;
            }

            _referenceDataCache.EPAOrganisations.TryGetValue(epaOrgId, out var epaOrganisations);

            return
                epaOrganisations == null
                ? false
                : epaOrganisations
                .Any(epa =>
                epa.Standard == stdCode.ToString()
                && epa.EffectiveFrom <= learnPlanEndDate
                && (epa.EffectiveTo ?? DateTime.MaxValue) >= learnPlanEndDate);
        }
    }
}
