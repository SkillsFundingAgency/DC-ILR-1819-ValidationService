using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class EsfEligibilityRule : IEsfEligibilityRule
    {
        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public bool Benefits { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public IEnumerable<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; set; }

        public IEnumerable<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; set; }

        public IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; set; }

        public IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; set; }
    }
}
