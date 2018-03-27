namespace ESFA.DC.ILR.ValidationService.ExternalData.Organisation.Interface
{
    public interface IOrganisationReferenceDataService
    {
        bool UkprnExists(long ukprn);
    }
}
