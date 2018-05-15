using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<T, U> : IRuleSetOrchestrationService<T, U>
        where T : class
    {
        private readonly IRuleSetResolutionService<T> _ruleSetResolutionService;
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;
        private readonly IPreValidationPopulationService<IValidationContext> _preValidationPopulationService;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService,
            IPreValidationPopulationService<IValidationContext> preValidationPopulationService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationOutputService<U> validationOutputService)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _validationItemProviderService = validationItemProviderService;
            _preValidationPopulationService = preValidationPopulationService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IValidationContext validationContext)
        {
            var ruleSet = _ruleSetResolutionService.Resolve().ToList();

            _preValidationPopulationService.Populate(validationContext);

            foreach (var validationItem in _validationItemProviderService.Provide())
            {
                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationOutputService.Process();
        }
    }
}
