using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ConsoleMessageValidationServiceStub<U> : IMessageValidationService<U>
    {
        private readonly IValidationErrorCache<U> _validationErrorCache;

        public ConsoleMessageValidationServiceStub(IValidationErrorCache<U> validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public IEnumerable<IValidationError> Execute(IMessage message)
        {
            var errors = new List<IValidationError>
            {
                BuildValidationError(ruleName: "SchemaError1", learnRefNumber: null, severity: Severity.Error),
                BuildValidationError(ruleName: "SchemaError2", learnRefNumber: null, severity: Severity.Error)
            };

            // do cache
            foreach (var error in errors)
            {
                _validationErrorCache.Add((U)error);
            }

            return errors;
        }

        public IValidationError BuildValidationError(string ruleName, string learnRefNumber, Severity? severity)
        {
            return new ValidationError(ruleName, learnRefNumber, severity: severity);
        }
    }
}
