using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationContext
    {
        IMessage Input { get; set; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorMessageLookupKey { get; }
    }

    public interface IPreValidationContext
    {
        string Input { get;  }

        string Output { get; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorMessageLookupKey { get; }

        string JobId { get; set; }
    }
}
