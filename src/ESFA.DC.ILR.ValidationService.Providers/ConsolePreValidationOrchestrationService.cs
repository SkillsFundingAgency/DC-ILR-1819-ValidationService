using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    /// <summary>
    /// This orchestration service will combine both Pre and actual validation orchestrations,
    /// this could be used for console app and FIS
    /// </summary>
    /// <typeparam name="U">The type.</typeparam>
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

        public async Task ExecuteAsync(
            IPreValidationContext preValidationContext,
            CancellationToken cancellationToken)
        {
            // get the file name
            _fileDataCache.FileName = preValidationContext.Input;

            // get ILR data from file
            await _preValidationPopulationService.PopulateAsync(cancellationToken);

            // xsd schema validations first; if failed then erturn.
            // TODO: Load only what is required in _preValidationPopulationService.Populate()
            if (_validateXMLSchemaService.Validate())
            {
                await _messageRuleSetOrchestrationService.Execute(cancellationToken);
                await _learnerRuleSetOrchestrationService.Execute(cancellationToken);
            }

            await _validationOutputService.ProcessAsync(CancellationToken.None);
        }
    }
}
