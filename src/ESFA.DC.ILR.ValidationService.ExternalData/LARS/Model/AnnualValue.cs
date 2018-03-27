using System;

namespace ESFA.DC.ILR.ValidationService.ExternalData.LARS.Model
{
    public class AnnualValue
    {
        public string LearnAimRef { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int? BasicSkills { get; set; }
    }
}
