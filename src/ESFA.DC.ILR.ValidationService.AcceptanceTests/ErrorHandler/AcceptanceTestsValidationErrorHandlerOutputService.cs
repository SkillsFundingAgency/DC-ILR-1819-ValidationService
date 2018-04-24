using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class AcceptanceTestsValidationErrorHandlerOutputService : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorHandler _validationErrorHandler;

        public AcceptanceTestsValidationErrorHandlerOutputService(IValidationErrorHandler validationErrorHandler)
        {
            _validationErrorHandler = validationErrorHandler;
        }

        public IEnumerable<IValidationError> Process()
        {
            IEnumerable<IValidationError> errors = ((ValidationErrorHandler)_validationErrorHandler).ErrorBag;
            return errors;
        }
    }
}
