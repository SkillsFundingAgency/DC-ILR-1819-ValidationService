namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationItemProviderService<out T>
    {
        T Provide();
    }
}
