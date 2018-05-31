using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    /// <summary>
    /// This orchestration service will combine both Pre and actual validation orchestrations,
    /// this could be used for console app and FIS
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class ConsolePreValidationOrchestrationService<T, U> : IPreValidationOrchestrationService<T, U>
        where T : class
    {
        private readonly IPreValidationPopulationService _preValidationPopulationService;
        private readonly ICache<IMessage> _messageCache;
        private readonly IRuleSetOrchestrationService<T, U> _ruleSetOrchestrationService;
        private readonly IValidationOutputService<U> _validationOutputService;

        public ConsolePreValidationOrchestrationService(
            IPreValidationPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            IRuleSetOrchestrationService<T, U> ruleSetOrchestrationService,
            IValidationOutputService<U> validationOutputService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _messageCache = messageCache;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IPreValidationContext preValidationContext)
        {
            // get ILR data from file
            _preValidationPopulationService.Populate();

            // get the learners
            var ilrMessage = _messageCache.Item;

            var validationContext = new ValidationContext()
            {
                Input = ilrMessage
            };

            _ruleSetOrchestrationService.Execute(validationContext);

            return _validationOutputService.Process();
        }
    }
}
