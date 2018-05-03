namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationContext
    {
        string Input { get; }

        string Output { get; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }
    }
}
