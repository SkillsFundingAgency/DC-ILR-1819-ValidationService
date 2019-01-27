using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public class LARSStandardFunding : ILARSStandardFunding
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public decimal? CoreGovContributionCap { get; set; }
    }
}
