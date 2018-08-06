using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<T, U> : IRuleSetOrchestrationService<T, U>
        where T : class
    {
        private readonly IRuleSetResolutionService<T> _ruleSetResolutionService;
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;
        private readonly IValidationErrorCache<U> _validationErrorCache;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationErrorCache<U> validationErrorCache)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _validationItemProviderService = validationItemProviderService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationErrorCache = validationErrorCache;
        }

        public IEnumerable<U> Execute(IValidationContext validationContext)
        {
            var ruleSet = _ruleSetResolutionService.Resolve().ToList();

            foreach (var validationItem in _validationItemProviderService.Provide())
            {
                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationErrorCache.ValidationErrors;
        }
    }
}
