using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSStandardFunding
    {
        DateTime EffectiveFrom { get; set; }

        DateTime? EffectiveTo { get; set; }

        decimal? CoreGovContributionCap { get; set; }
    }
}
