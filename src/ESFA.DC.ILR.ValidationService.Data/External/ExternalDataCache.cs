using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External
{
    /// <summary>
    /// the external data cache implementation
    /// </summary>
    /// <seealso cref="IExternalDataCache" />
    public class ExternalDataCache :
        IExternalDataCache
    {
        public IReadOnlyCollection<long> ULNs { get; set; }

        /// <summary>
        /// Gets or sets the employer reference numbers.
        /// </summary>
        public IReadOnlyCollection<int> ERNs { get; set; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; set; }

        public IReadOnlyCollection<Framework> Frameworks { get; set; }

        public IReadOnlyDictionary<long, Organisation.Model.Organisation> Organisations { get; set; }

        /// <summary>
        /// Gets or sets the LARS standard validities.
        /// </summary>
        public IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; set; }

        /// <summary>
        /// Gets or sets the epa organisations.
        /// </summary>
        public IReadOnlyCollection<IEPAOrganisation> EPAOrganisations { get; set; }

        public IReadOnlyCollection<string> Postcodes { get; set; }

        /// <summary>
        /// Gets or sets the ons postcodes.
        /// </summary>
        public IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; set; }

        public IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; set; }

        /// <summary>
        /// Gets or sets the FCS contracts.
        /// </summary>
        public IReadOnlyCollection<IFcsContract> FCSContracts { get; set; }

        /// <summary>
        /// Gets or sets the FCS contract allocations.
        /// </summary>
        public IReadOnlyCollection<IFcsContractAllocation> FCSContractAllocations { get; set; }

        /// <summary>
        /// Gets or sets the esf eligibility rule employment statuses.
        /// </summary>
        public IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> ESFEligibilityRuleEmploymentStatuses { get; set; }

        /// <summary>
        /// Gets or sets the esf eligibility rule local authorities.
        /// </summary>
        public IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> ESFEligibilityRuleLocalAuthorities { get; set; }

        /// <summary>
        /// Gets or sets the ESF eligibility rule enterprise partnerships
        /// </summary>
        public IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> ESFEligibilityRuleEnterprisePartnerships { get; set; }

        /// <summary>
        /// Gets or sets the esf eligibility rule sector subject area levels.
        /// </summary>
        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> EsfEligibilityRuleSectorSubjectAreaLevels { get; set; }
    }
}
