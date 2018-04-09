namespace ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface
{
    public interface IULNReferenceDataService
    {
        bool Exists(long? uln);
    }
}
