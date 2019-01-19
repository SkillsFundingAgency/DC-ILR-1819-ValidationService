using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class EsfEligibilityRule : IEsfEligibilityRule
    {
        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public bool? Benefits { get; set; }

        public IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; set; }

        public IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; set; }

        public IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; set; }

        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; set; }
    }
}
