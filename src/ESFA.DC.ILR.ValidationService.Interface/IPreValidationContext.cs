namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationContext
    {
        string Input { get; }

        string Output { get; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorMessageLookupKey { get; }

        string JobId { get; }
    }
}
