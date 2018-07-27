using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class AnnualValue
    {
        public string LearnAimRef { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public int? BasicSkills { get; set; }

        public int? FullLevel2EntitlementCategory { get; set; }

        public int? FullLevel3EntitlementCategory { get; set; }

        public decimal? FullLevel3Percent { get; set; }
    }
}
