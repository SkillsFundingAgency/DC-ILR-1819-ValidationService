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
    public class ConsolePreValidationOrchestrationService<U> : IPreValidationOrchestrationService<U>
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly ICache<IMessage> _messageCache;
        private readonly IRuleSetOrchestrationService<ILearner, U> _learnerRuleSetOrchestrationService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _messageRuleSetOrchestrationService;
        private readonly IValidationOutputService<U> _validationOutputService;

        public ConsolePreValidationOrchestrationService(
            IPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            IRuleSetOrchestrationService<ILearner, U> learnerRuleSetOrchestrationService,
            IRuleSetOrchestrationService<IMessage, U> messageRuleSetOrchestrationService,
            IValidationOutputService<U> validationOutputService)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _messageCache = messageCache;
            _learnerRuleSetOrchestrationService = learnerRuleSetOrchestrationService;
            _messageRuleSetOrchestrationService = messageRuleSetOrchestrationService;
            _validationOutputService = validationOutputService;
        }

        public IEnumerable<U> Execute(IPreValidationContext preValidationContext)
        {
            // get ILR data from file
            _preValidationPopulationService.Populate();

            // get the learners
            var ilrMessage = _messageCache.Item;

            _messageRuleSetOrchestrationService.Execute();
            _learnerRuleSetOrchestrationService.Execute();

            return _validationOutputService.Process();
        }
    }
}
