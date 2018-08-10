using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageValidationServiceStub<U> : IMessageValidationService<U>
    {
        public MessageValidationServiceStub()
        {
        }

        public IEnumerable<IValidationError> Execute(IMessage message)
        {
            return new List<IValidationError>
            {
                BuildValidationError(ruleName: "SchemaError1", learnRefNumber: null, severity: Severity.Error),
                BuildValidationError(ruleName: "SchemaError2", learnRefNumber: null, severity: Severity.Error)
            };
        }

        public IValidationError BuildValidationError(string ruleName, string learnRefNumber, Severity? severity)
        {
            return new ValidationError(ruleName, learnRefNumber, severity: severity);
        }
    }
}
