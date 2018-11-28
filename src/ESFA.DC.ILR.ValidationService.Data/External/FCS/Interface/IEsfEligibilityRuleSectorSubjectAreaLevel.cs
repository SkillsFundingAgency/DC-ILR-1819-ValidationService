namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    public interface IEsfEligibilityRuleSectorSubjectAreaLevel
    {
        int Id { get; set; }

        string LotReference { get; set; }

        string MaxLevelCode { get; set; }

        string MinLevelCode { get; set; }

        decimal? SectorSubjectAreaCode { get; set; }

        string TenderSpecReference { get; set; }
    }
}