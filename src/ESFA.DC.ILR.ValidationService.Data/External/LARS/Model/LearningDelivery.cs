using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class LearningDelivery
    {
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }   
        public int? FrameworkCommonComponent { get; set; }
        public IEnumerable<LearningDeliveryCategory> LearningDeliveryCategories { get; set; }
        public IEnumerable<FrameworkAim> FrameworkAims { get; set; }
        public IEnumerable<AnnualValue> AnnualValues { get; set; }
    }
}
