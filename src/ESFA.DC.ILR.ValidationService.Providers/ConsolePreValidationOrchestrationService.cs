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
    public class ConsolePreValidationOrchestrationService<T, U> : IPreValidationOrchestrationService<U>
        where T : class
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly ICache<IMessage> _messageCache;
        private readonly IMessageValidationService<U> _messageValidationService;
        private readonly IRuleSetOrchestrationService<T, U> _ruleSetOrchestrationService;
        private readonly IValidationOutputService<U> _validationOutputService;

        public ConsolePreValidationOrchestrationService(
            IPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            IMessageValidationService<U> messageValidationService,
            IRuleSetOrchestrationService<T, U> ruleSetOrchestrationService,
            IValidationOutputService<U> validationOutputService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _messageCache = messageCache;
            _messageValidationService = messageValidationService;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IPreValidationContext preValidationContext)
        {
            // get ILR data from file
            _preValidationPopulationService.Populate();

            // get the learners
            var ilrMessage = _messageCache.Item;

            _messageValidationService.Execute(ilrMessage);
            _ruleSetOrchestrationService.Execute();

            return _validationOutputService.Process();
        }
    }
}
