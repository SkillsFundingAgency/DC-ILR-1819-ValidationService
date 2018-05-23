using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorHandlerOutputService : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorCache _validationErrorCache;

        public ValidationErrorHandlerOutputService(IValidationErrorCache validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public IEnumerable<IValidationError> Process()
        {
            IEnumerable<IValidationError> errors = _validationErrorCache.ValidationErrors;

            return errors;
        }
    }
}
