﻿using System;

namespace ESFA.DC.ILR.ValidationService.ExternalData.LARS.Model
{
    public class LearningDeliveryCategory
    {
        public string LearnAimRef { get; set; }        
        public int CategoryRef { get; set; }        
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }        
        public LearningDelivery LearningDelivery { get; set; }
    }
}
