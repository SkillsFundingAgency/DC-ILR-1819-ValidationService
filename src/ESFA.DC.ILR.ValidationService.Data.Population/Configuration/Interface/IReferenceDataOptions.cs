namespace ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface
{
    public interface IReferenceDataOptions
    {
        string LARSConnectionString { get; }

        string OrganisationsConnectionString { get; }

        string PostcodesConnectionString { get; }

        string ULNConnectionstring { get; }

        string FCSConnectionString { get; }

        string EPAConnectionString { get; }

        string ValidationErrorsConnectionString { get; }
    }
}
