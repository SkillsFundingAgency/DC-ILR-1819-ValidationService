using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class LearningDeliveryCategory
    {
        public string LearnAimRef { get; set; }

        public int CategoryRef { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
