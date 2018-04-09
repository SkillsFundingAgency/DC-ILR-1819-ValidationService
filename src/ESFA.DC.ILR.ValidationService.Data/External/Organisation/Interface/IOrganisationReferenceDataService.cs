namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface
{
    public interface IOrganisationReferenceDataService
    {
        bool UkprnExists(long ukprn);
    }
}
