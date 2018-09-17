using System.Collections.Generic;
using System.Threading;
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
        private readonly IValidateXMLSchemaService _validateXMLSchemaService;
        private readonly IPopulationService _preValidationPopulationService;
        private readonly ICache<IMessage> _messageCache;
        private readonly IRuleSetOrchestrationService<ILearner, U> _learnerRuleSetOrchestrationService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _messageRuleSetOrchestrationService;
        private readonly IValidationOutputService<U> _validationOutputService;
        private readonly IFileDataCache _fileDataCache;

        public ConsolePreValidationOrchestrationService(
            IValidateXMLSchemaService validateXMLSchemaService,
            IPopulationService preValidationPopulationService,
            ICache<IMessage> messageCache,
            IRuleSetOrchestrationService<ILearner, U> learnerRuleSetOrchestrationService,
            IRuleSetOrchestrationService<IMessage, U> messageRuleSetOrchestrationService,
            IValidationOutputService<U> validationOutputService,
            IFileDataCache fileDataCache)
        {
            _validateXMLSchemaService = validateXMLSchemaService;
            _preValidationPopulationService = preValidationPopulationService;
            _messageCache = messageCache;
            _learnerRuleSetOrchestrationService = learnerRuleSetOrchestrationService;
            _messageRuleSetOrchestrationService = messageRuleSetOrchestrationService;
            _validationOutputService = validationOutputService;
            _fileDataCache = fileDataCache;
        }

        public IEnumerable<U> Execute(IPreValidationContext preValidationContext, CancellationToken cancellationToken)
        {
            // get the file name
            _fileDataCache.FileName = preValidationContext.Input;

            // get ILR data from file
            _preValidationPopulationService.Populate();

            // xsd schema validations first; if failed then erturn.
            // TODO: Load only what is required in _preValidationPopulationService.Populate()
            if (_validateXMLSchemaService.Validate())
            {
                // get the learners
                var ilrMessage = _messageCache.Item;

                _messageRuleSetOrchestrationService.Execute(CancellationToken.None);
                _learnerRuleSetOrchestrationService.Execute(CancellationToken.None);
            }

            return _validationOutputService.Process();
        }
    }
}
