using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<T, U> : IRuleSetOrchestrationService<T, U>
        where T : class
    {
        private readonly IRuleSetResolutionService<T> _ruleSetResolutionService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;
        private readonly IExternalDataCachePopulationService<T> _referenceDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachPopulationService;
        private readonly IValidationOutputService<U> _validationOutputService;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService,
            IExternalDataCachePopulationService<T> referenceDataCachePopulationService,
            IInternalDataCachePopulationService internalDataCachePopulationService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationOutputService<U> validationOutputService)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _validationItemProviderService = validationItemProviderService;
            _referenceDataCachePopulationService = referenceDataCachePopulationService;
            _internalDataCachPopulationService = internalDataCachePopulationService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IValidationContext validationContext)
        {
            var ruleSet = _ruleSetResolutionService.Resolve().ToList();

            var validationItems = _validationItemProviderService.Provide(validationContext);

            _referenceDataCachePopulationService.Populate(validationItems);
            _internalDataCachPopulationService.Populate();

            foreach (var validationItem in validationItems)
            {
                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationOutputService.Process();
        }
    }
}
