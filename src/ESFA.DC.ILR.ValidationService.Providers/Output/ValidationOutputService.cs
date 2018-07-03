using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidationOutputService : IValidationOutputService<IValidationError>
    {
        private const string Error = "E";
        private const string Warning = "W";

        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly IKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IPreValidationContext _validationContext;
        private readonly IJsonSerializationService _serializationService;
        private readonly IValidationErrorsDataService _validationErrorsDataService;

        public ValidationOutputService(
            IValidationErrorCache<IValidationError> validationErrorCache,
            ICache<IMessage> messageCache,
            IKeyValuePersistenceService keyValuePersistenceService,
            IPreValidationContext validationContext,
            IJsonSerializationService serializationService,
            IValidationErrorsDataService validationErrorsDataService)
        {
            _validationErrorCache = validationErrorCache;
            _messageCache = messageCache;
            _keyValuePersistenceService = keyValuePersistenceService;
            _validationContext = validationContext;
            _serializationService = serializationService;
            _validationErrorsDataService = validationErrorsDataService;
        }

        public IEnumerable<IValidationError> Process()
        {
            var invalidLearnerRefNumbers = BuildInvalidLearnRefNumbers().ToList();
            var validLearnerRefNumbers = BuildValidLearnRefNumbers(invalidLearnerRefNumbers).ToList();

            var validationErrors = _validationErrorCache
                .ValidationErrors
                .Select(ve => new ValidationError()
                {
                    LearnerReferenceNumber = ve.LearnerReferenceNumber,
                    AimSequenceNumber = ve.AimSequenceNumber,
                    RuleName = ve.RuleName,
                    Severity = SeverityToString(ve.Severity),
                    ValidationErrorParameters = ve.ErrorMessageParameters?
                    .Select(emp => new ValidationErrorParameter()
                        {
                            PropertyName = emp.PropertyName,
                            Value = emp.Value
                        }).ToList()
                }).ToList();

            //var validationErrorMessageLookups = _validationErrorCache
            //    .ValidationErrors
            //    .Select(ve => ve.RuleName)
            //    .Distinct()
            //    .Select(rn => new ValidationErrorMessageLookup()
            //    {
            //        RuleName = rn,
            //        Message = _validationErrorsDataService.MessageforRuleName(rn)
            //    }).ToList();

            SaveAsync(validLearnerRefNumbers, invalidLearnerRefNumbers, validationErrors).Wait();

            return _validationErrorCache.ValidationErrors;
        }

        public IEnumerable<string> BuildInvalidLearnRefNumbers()
        {
            return _validationErrorCache
                .ValidationErrors
                .Select(ve => ve.LearnerReferenceNumber)
                .Distinct();
        }

        public IEnumerable<string> BuildValidLearnRefNumbers(IEnumerable<string> invalidLearnRefNumbers)
        {
            var invalidLearnRefNumbersHashSet = new HashSet<string>(invalidLearnRefNumbers);

            return _messageCache
                .Item
                .Learners
                .Select(l => l.LearnRefNumber)
                .Where(lrn => !invalidLearnRefNumbersHashSet.Contains(lrn));
        }

        public async Task SaveAsync(IEnumerable<string> validLearnerRefNumbers, IEnumerable<string> invalidLearnerRefNumbers, IEnumerable<ValidationError> validationErrors)
        {
            var validLearnRefNumbersKey = _validationContext.ValidLearnRefNumbersKey;
            var invalidLearnRefNumbersKey = _validationContext.InvalidLearnRefNumbersKey;
            var validationErrorsKey = _validationContext.ValidationErrorsKey;

            var validationContext = _validationContext;
            validationContext.InvalidLearnRefNumbersCount = invalidLearnerRefNumbers.Count();
            validationContext.ValidLearnRefNumbersCount = validLearnerRefNumbers.Count();
            validationContext.ValidationTotalErrorCount = validationErrors.Count(x => x.Severity == Error);
            validationContext.ValidationTotalWarningCount = validationErrors.Count(x => x.Severity == Warning);

            await Task.WhenAll(
                new[]
                {
                    _keyValuePersistenceService.SaveAsync(validLearnRefNumbersKey, _serializationService.Serialize(validLearnerRefNumbers)),
                    _keyValuePersistenceService.SaveAsync(invalidLearnRefNumbersKey, _serializationService.Serialize(invalidLearnerRefNumbers)),
                    _keyValuePersistenceService.SaveAsync(validationErrorsKey, _serializationService.Serialize(validationErrors)),
                });
        }

        public string SeverityToString(Severity? severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return Error;
                case Severity.Warning:
                    return Warning;
                case null:
                    return null;
                default:
                    return null;
            }
        }
    }
}
