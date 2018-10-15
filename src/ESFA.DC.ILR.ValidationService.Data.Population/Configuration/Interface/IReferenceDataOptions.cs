namespace ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface
{
    public interface IReferenceDataOptions
    {
        string LARSConnectionString { get; }

        string OrganisationsConnectionString { get; }

        string PostcodesConnectionString { get; }

        string ULNConnectionstring { get; }

        string FCSConnectionstring { get; }

        string ValidationErrorsConnectionString { get; }
    }
}
