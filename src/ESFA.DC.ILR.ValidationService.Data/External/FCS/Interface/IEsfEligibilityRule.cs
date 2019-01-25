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

        bool Benefits { get; }

        IEnumerable<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; }

        IEnumerable<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; }

        IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; }

        IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; }
    }
}
