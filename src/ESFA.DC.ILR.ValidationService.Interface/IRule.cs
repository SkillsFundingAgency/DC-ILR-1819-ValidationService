namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRule<in T> where T : class
    {
        void Validate(T objectToValidate);
    }
}