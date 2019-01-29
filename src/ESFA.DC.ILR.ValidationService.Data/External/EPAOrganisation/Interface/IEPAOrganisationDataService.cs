using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface
{
    public interface IEPAOrganisationDataService
    {
        bool IsValidEpaOrg(string epaOrgId, int? stdCode, DateTime learnPlanEndDate);
    }
}
