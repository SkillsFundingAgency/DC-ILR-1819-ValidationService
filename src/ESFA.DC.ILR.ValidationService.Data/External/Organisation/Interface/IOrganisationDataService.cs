﻿namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface
{
    public interface IOrganisationDataService
    {
        bool UkprnExists(long ukprn);

        bool LegalOrgTypeMatchForUkprn(long ukprn, string legalOrgType);

        bool IsPartnerUkprn(long ukprn);

        string GetLegalOrgTypeForUkprn(long ukprn);

        bool CampIdExists(string campId);

        bool CampIdMatchForUkprn(string campId, long ukprn);
    }
}
