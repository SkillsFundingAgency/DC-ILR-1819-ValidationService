using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class EsfEligibilityRuleSectorSubjectAreaLevel : IEsfEligibilityRuleSectorSubjectAreaLevel
    {
        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public decimal? SectorSubjectAreaCode { get; set; }

        public string MinLevelCode { get; set; }

        public string MaxLevelCode { get; set; }
    }
}
