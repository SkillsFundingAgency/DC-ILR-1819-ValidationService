using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule definition
    /// </summary>
    public interface IEsfEligibilityRule
    {
        string TenderSpecReference { get; }

        string LotReference { get; }

        bool? Benefits { get; }

        IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; }

        IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; }

        IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; }

        IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; }
    }
}
