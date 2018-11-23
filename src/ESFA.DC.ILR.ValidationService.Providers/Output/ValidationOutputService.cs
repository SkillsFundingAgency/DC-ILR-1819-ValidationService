using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidationOutputService : IValidationOutputService<IValidationError>
    {
        private const string Error = "E";
        private const string Warning = "W";
        private const string Fail = "F";

        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly IKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IPreValidationContext _validationContext;
        private readonly IJsonSerializationService _serializationService;
        private readonly IValidationErrorsDataService _validationErrorsDataService;
        private readonly ILogger _logger;

        public ValidationOutputService(
            IValidationErrorCache<IValidationError> validationErrorCache,
            ICache<IMessage> messageCache,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService keyValuePersistenceService,
            IPreValidationContext validationContext,
            IJsonSerializationService serializationService,
            IValidationErrorsDataService validationErrorsDataService,
            ILogger logger)
        {
            _validationErrorCache = validationErrorCache;
            _messageCache = messageCache;
            _keyValuePersistenceService = keyValuePersistenceService;
            _validationContext = validationContext;
            _serializationService = serializationService;
            _validationErrorsDataService = validationErrorsDataService;
            _logger = logger;
        }

        public async Task<IEnumerable<IValidationError>> ProcessAsync(CancellationToken cancellationToken)
        {
            var existingValidationErrors = await GetExistingValidationErrors();

            var validationErrors = _validationErrorCache
                .ValidationErrors
                .Select(ve => new ValidationError
                {
                    LearnerReferenceNumber = ve.LearnerReferenceNumber,
                    AimSequenceNumber = ve.AimSequenceNumber,
                    RuleName = ve.RuleName,
                    Severity = SeverityToString(ve.Severity),
                    ValidationErrorParameters = ve.ErrorMessageParameters?
                    .Select(emp => new ValidationErrorParameter
                    {
                        PropertyName = emp.PropertyName,
                        Value = emp.Value
                    }).ToList()
                }).ToList();

            validationErrors.AddRange(existingValidationErrors);

            var invalidLearnerRefNumbers = BuildInvalidLearnRefNumbers(validationErrors).ToList();
            var validLearnerRefNumbers = BuildValidLearnRefNumbers(invalidLearnerRefNumbers, validationErrors).ToList();
            _logger.LogDebug($"ValidationOutputService invalid:{invalidLearnerRefNumbers.Count} valid:{validLearnerRefNumbers.Count}");

            var validationErrorMessageLookups = validationErrors
                .Select(ve => ve.RuleName)
                .Distinct()
                .Select(rn => new ValidationErrorMessageLookup
                {
                    RuleName = rn,
                    Message = _validationErrorsDataService.MessageforRuleName(rn)
                }).ToList();

            await SaveAsync(
                validLearnerRefNumbers,
                invalidLearnerRefNumbers,
                validationErrors,
                validationErrorMessageLookups,
                cancellationToken);

            return _validationErrorCache.ValidationErrors;
        }

        public IEnumerable<string> BuildInvalidLearnRefNumbers(IEnumerable<ValidationError> validationErrors)
        {
            return validationErrors
                .Where(x => !string.IsNullOrEmpty(x.LearnerReferenceNumber)
                    && x.Severity == Error)
                .Select(ve => ve.LearnerReferenceNumber)
                .Distinct();
        }

        public IEnumerable<string> BuildValidLearnRefNumbers(IEnumerable<string> invalidLearnRefNumbers, IEnumerable<ValidationError> validationErrors)
        {
            var invalidLearnRefNumbersHashSet = new HashSet<string>(invalidLearnRefNumbers);
            _logger.LogDebug($"BuildValidLearnRefNumbers {invalidLearnRefNumbersHashSet.Count}");
            if (validationErrors.Any(x => !string.IsNullOrEmpty(x.LearnerReferenceNumber)))
            {
                return _messageCache
                    .Item
                    .Learners
                    .Select(l => l.LearnRefNumber)
                    .Where(lrn => !invalidLearnRefNumbersHashSet.Contains(lrn));
            }

            _logger.LogDebug($"BuildValidLearnRefNumbers: no errors in validationErrorCache have learner ref numbers {_messageCache.Item.Learners.Count}");
            return _messageCache
                    .Item
                    .Learners
                    .Select(l => l.LearnRefNumber).Distinct();
        }

        public async Task SaveAsync(
            IEnumerable<string> validLearnerRefNumbers,
            IEnumerable<string> invalidLearnerRefNumbers,
            IEnumerable<ValidationError> validationErrors,
            IEnumerable<ValidationErrorMessageLookup> validationErrorMessageLookups,
            CancellationToken cancellationToken)
        {
            var validLearnRefNumbersKey = _validationContext.ValidLearnRefNumbersKey;
            var invalidLearnRefNumbersKey = _validationContext.InvalidLearnRefNumbersKey;
            var validationErrorsKey = _validationContext.ValidationErrorsKey;
            var validationErrorMessageLookupKey = _validationContext.ValidationErrorMessageLookupKey;

            var validationContext = _validationContext;
            validationContext.InvalidLearnRefNumbersCount = invalidLearnerRefNumbers.Count();
            validationContext.ValidLearnRefNumbersCount = validLearnerRefNumbers.Count();
            validationContext.ValidationTotalErrorCount = validationErrors.Count(x => x.Severity == Error);
            validationContext.ValidationTotalWarningCount = validationErrors.Count(x => x.Severity == Warning);

            await Task.WhenAll(
                _keyValuePersistenceService.SaveAsync(validLearnRefNumbersKey, _serializationService.Serialize(validLearnerRefNumbers), cancellationToken),
                _keyValuePersistenceService.SaveAsync(invalidLearnRefNumbersKey, _serializationService.Serialize(invalidLearnerRefNumbers), cancellationToken),
                _keyValuePersistenceService.SaveAsync(validationErrorsKey, _serializationService.Serialize(validationErrors), cancellationToken),
                _keyValuePersistenceService.SaveAsync(validationErrorMessageLookupKey, _serializationService.Serialize(validationErrorMessageLookups), cancellationToken));
        }

        public string SeverityToString(Severity? severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return Error;
                case Severity.Warning:
                    return Warning;
                case Severity.Fail:
                    return Fail;
                case null:
                    return null;
                default:
                    return null;
            }
        }

        private async Task<IEnumerable<ValidationError>> GetExistingValidationErrors()
        {
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>();

            try
            {
                validationErrors = _serializationService.Deserialize<List<ValidationError>>(await _keyValuePersistenceService.GetAsync(_validationContext.ValidationErrorsKey));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed To get Existing Validation Errors, assume none available and carry on.", e);
            }

            return validationErrors;
        }
    }
}
