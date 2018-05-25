using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors
{
    public class ValidationErrorsDataService : IValidationErrorsDataService
    {
        public Severity? SeverityForRuleName(string ruleName)
        {
            return Severity.Error;
        }
    }
}
