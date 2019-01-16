using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// the external data cache definition
    /// </summary>
    public interface IExternalDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        /// <summary>
        /// Gets the employer reference numbers.
        /// </summary>
        IReadOnlyCollection<int> ERNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<Framework> Frameworks { get; }

        /// <summary>
        /// Gets the LARS standards.
        /// </summary>
        IReadOnlyCollection<ILARSStandard> Standards { get; }

        /// <summary>
        /// Gets the LARS standard validities.
        /// </summary>
        IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; }

        IReadOnlyDictionary<long, Organisation> Organisations { get; }

        /// <summary>
        /// Gets the epa organisations.
        /// </summary>
        IReadOnlyCollection<IEPAOrganisation> EPAOrganisations { get; }

        IReadOnlyCollection<string> Postcodes { get; }

        /// <summary>
        /// Gets the ons postcodes.
        /// </summary>
        IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; }

        IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Gets the FCS contract allocations.
        /// </summary>
        IReadOnlyCollection<IFcsContractAllocation> FCSContractAllocations { get; }

        /// <summary>
        /// Gets the ESF eligibility rule employment statuses.
        /// </summary>
        IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> ESFEligibilityRuleEmploymentStatuses { get; }

        /// <summary>
        /// Gets the esf eligibility rule local authorities.
        /// </summary>
        IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> ESFEligibilityRuleLocalAuthorities { get; }

        /// <summary>
        /// Gets the ESF eligibility rule enterprise partnerships
        /// </summary>
        IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> ESFEligibilityRuleEnterprisePartnerships { get; }

        /// <summary>
        /// Gets the ESF eligibility rule subject area level codes.
        /// </summary>
        IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> EsfEligibilityRuleSectorSubjectAreaLevels { get; }
    }
}
