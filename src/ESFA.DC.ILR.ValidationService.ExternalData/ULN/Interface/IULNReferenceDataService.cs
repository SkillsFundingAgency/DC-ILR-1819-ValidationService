namespace ESFA.DC.ILR.ValidationService.ExternalData.ULN.Interface
{
    public interface IULNReferenceDataService
    {
        bool Exists(long? uln);
    }
}
