using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_27Rule : IDerivedData_27Rule
    {
        private readonly string[] _legalOrgTypes = { "USFC", "USDC", "USAH", "USAD", "UGFE", "UHEO", "ULEA", "ULAD", "USCL", "LAFB", "UFES", "SSPS" };

        private readonly IOrganisationDataService _organisationDataService;

        public DerivedData_27Rule(IOrganisationDataService organisationDataService)
        {
            _organisationDataService = organisationDataService;
        }

        public bool IsUKPRNCollegeOrGrantFundedProvider(int ukprn) => _legalOrgTypes.Contains(_organisationDataService.GetLegalOrgTypeForUkprn(ukprn));
    }
}
