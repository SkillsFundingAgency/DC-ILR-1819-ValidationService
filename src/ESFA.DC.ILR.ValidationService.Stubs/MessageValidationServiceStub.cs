using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class MessageValidationServiceStub<U> : IMessageValidationService<U>
    {
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IRuleSetResolutionService<IMessage> _ruleSetResolutionService;
        private readonly IRuleSetExecutionService<IMessage> _ruleSetExecutionService;

        public MessageValidationServiceStub(
            IValidationErrorCache<U> validationErrorCache,
            IRuleSetResolutionService<IMessage> ruleSetResolutionService,
            IRuleSetExecutionService<IMessage> ruleSetExecutionService)
        {
            _validationErrorCache = validationErrorCache;
            _ruleSetResolutionService = ruleSetResolutionService;
            _ruleSetExecutionService = ruleSetExecutionService;
        }

        public IEnumerable<U> Execute(IMessage message)
        {
            var errors = ExecuteRuleSet(message);

            return errors;
        }

        public IEnumerable<U> ExecuteRuleSet(IMessage message)
        {
            var ruleSet = _ruleSetResolutionService.Resolve().ToList();

            _ruleSetExecutionService.Execute(ruleSet, message);

            return _validationErrorCache.ValidationErrors;
        }

        public IValidationError BuildValidationError(string ruleName, string learnRefNumber, Severity? severity)
        {
            return new ValidationError(ruleName, learnRefNumber, severity: severity);
        }
    }
}
