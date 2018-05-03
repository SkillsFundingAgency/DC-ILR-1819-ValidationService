using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorHandler : IValidationErrorHandler
    {
        private readonly IValidationErrorCache _validationErrorCache;

        public ValidationErrorHandler(IValidationErrorCache validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public void Handle(string ruleName, string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null)
        {
            _validationErrorCache.Add(BuildValidationError(ruleName, learnRefNumber, aimSequenceNumber, errorMessageParameters));
        }

        public IValidationError BuildValidationError(string ruleName, string learnRefNumber, long? aimSequenceNumber, IEnumerable<IErrorMessageParameter> errorMessageParameters)
        {
            return new ValidationError(ruleName, learnRefNumber, aimSequenceNumber, errorMessageParameters);
        }

        public IErrorMessageParameter BuildErrorMessageParameter(string propertyName, object value)
        {
            return new ErrorMessageParameter(propertyName, value?.ToString());
        }
    }
}