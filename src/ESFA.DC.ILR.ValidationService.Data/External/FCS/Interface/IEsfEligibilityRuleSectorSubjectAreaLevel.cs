namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    public interface IEsfEligibilityRuleSectorSubjectAreaLevel
    {
        string LotReference { get; }

        string MaxLevelCode { get; }

        string MinLevelCode { get; }

        decimal? SectorSubjectAreaCode { get; }

        string TenderSpecReference { get; }
    }
}