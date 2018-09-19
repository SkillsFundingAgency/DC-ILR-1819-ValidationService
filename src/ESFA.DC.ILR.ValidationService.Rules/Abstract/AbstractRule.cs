using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Abstract
{
    public abstract class AbstractRule
    {
        private readonly IValidationErrorHandler _validationErrorHandler;

        private readonly string _ruleName;

        protected AbstractRule(IValidationErrorHandler validationErrorHandler, string ruleName)
        {
            _validationErrorHandler = validationErrorHandler;
            _ruleName = ruleName;
        }

        public string RuleName
        {
            get { return _ruleName; }
        }

        protected void HandleValidationError(string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null)
        {
            _validationErrorHandler.Handle(_ruleName, learnRefNumber, aimSequenceNumber, errorMessageParameters);
        }

        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, object value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, value);
        }

        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, value.ToString("d", new CultureInfo("en-GB")));
        }

        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime? value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, value?.ToString("d", new CultureInfo("en-GB")));
        }
    }
}
