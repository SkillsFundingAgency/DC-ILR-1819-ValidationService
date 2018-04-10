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
        private readonly IMessageCachePopulationService _messageCachePopulationService;
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;
        private readonly IExternalDataCachePopulationService<T> _externalDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachPopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IMessageCachePopulationService messageCachePopulationService,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService,
            IExternalDataCachePopulationService<T> referenceDataCachePopulationService,
            IInternalDataCachePopulationService internalDataCachePopulationService,
            IFileDataCachePopulationService fileDataCachePopulationService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationOutputService<U> validationOutputService)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _messageCachePopulationService = messageCachePopulationService;
            _validationItemProviderService = validationItemProviderService;
            _externalDataCachePopulationService = referenceDataCachePopulationService;
            _internalDataCachPopulationService = internalDataCachePopulationService;
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IValidationContext validationContext)
        {
            var ruleSet = _ruleSetResolutionService.Resolve().ToList();

            _messageCachePopulationService.Populate();

            var validationItems = _validationItemProviderService.Provide().ToList();

            _externalDataCachePopulationService.Populate(validationItems);
            _internalDataCachPopulationService.Populate();
            _fileDataCachePopulationService.Populate();

            foreach (var validationItem in validationItems)
            {
                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationOutputService.Process();
        }
    }
}
