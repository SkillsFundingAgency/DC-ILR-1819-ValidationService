﻿using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;

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