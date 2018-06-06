namespace ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface
{
    public interface IReferenceDataOptions
    {
        string LARSConnectionString { get; }

        string PostcodesConnectionString { get; }

        string ULNConnectionstring { get; }

        string ValidationErrorsConnectionString { get; }
    }
}
