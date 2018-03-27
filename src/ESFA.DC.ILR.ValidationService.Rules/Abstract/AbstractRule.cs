using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Abstract
{
    public abstract class AbstractRule
    {
        private readonly IValidationErrorHandler _validationErrorHandler;

        protected AbstractRule(IValidationErrorHandler validationErrorHandler)
        {
            _validationErrorHandler = validationErrorHandler;
        }

        protected void HandleValidationError(string ruleName, string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<string> errorMessageParameters = null)
        {
            _validationErrorHandler.Handle(ruleName, learnRefNumber, aimSequenceNumber, errorMessageParameters);
        }
    }
}
