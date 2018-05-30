using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ValidationOutputServiceStub : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly IKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IPreValidationContext _validationContext;
        private readonly IJsonSerializationService _serializationService;

        public ValidationOutputServiceStub(
            IValidationErrorCache<IValidationError> validationErrorCache,
            ICache<IMessage> messageCache,
            IKeyValuePersistenceService keyValuePersistenceService,
            IPreValidationContext validationContext,
            IJsonSerializationService serializationService)
        {
            _validationErrorCache = validationErrorCache;
            _messageCache = messageCache;
            _keyValuePersistenceService = keyValuePersistenceService;
            _validationContext = validationContext;
            _serializationService = serializationService;
        }

        public IEnumerable<IValidationError> Process()
        {
            var invalidLearnerRefNumbers = new HashSet<string>(_validationErrorCache.ValidationErrors.Select(ve => ve.LearnerReferenceNumber).Distinct());

            var validLearnerRefNumbers = _messageCache.Item.Learners.Select(l => l.LearnRefNumber).Where(lrn => !invalidLearnerRefNumbers.Contains(lrn)).ToList();

            var validationErrors = _validationErrorCache
                .ValidationErrors
                .Select(ve => new ValidationError()
                {
                    LearnerReferenceNumber = ve.LearnerReferenceNumber,
                    AimSequenceNumber = ve.AimSequenceNumber,
                    RuleName = ve.RuleName,
                    Severity = SeverityToString(ve.Severity),
                    ValidationErrorParameters = ve.ErrorMessageParameters
                    .Select(emp => new ValidationErrorParameter()
                        {
                            PropertyName = emp.PropertyName,
                            Value = emp.Value
                        }).ToList()
                }).ToList();

            var validationErrorMessageLookups = _validationErrorCache
                .ValidationErrors
                .Select(ve => ve.RuleName)
                .Distinct()
                .Select(rn => new ValidationErrorMessageLookup()
                {
                    RuleName = rn,
                    Message = "Placeholder"
                }).ToList();

            _keyValuePersistenceService.SaveAsync(_validationContext.ValidLearnRefNumbersKey, _serializationService.Serialize(validLearnerRefNumbers)).Wait();
            _keyValuePersistenceService.SaveAsync(_validationContext.InvalidLearnRefNumbersKey, _serializationService.Serialize(invalidLearnerRefNumbers)).Wait();
            _keyValuePersistenceService.SaveAsync(_validationContext.ValidationErrorsKey, _serializationService.Serialize(validationErrors)).Wait();
            _keyValuePersistenceService.SaveAsync(_validationContext.ValidationErrorMessageLookupKey, _serializationService.Serialize(validationErrorMessageLookups)).Wait();

            var validStored = _keyValuePersistenceService.GetAsync(_validationContext.ValidLearnRefNumbersKey).Result;
            var invalidStored = _keyValuePersistenceService.GetAsync(_validationContext.InvalidLearnRefNumbersKey).Result;
            var validationErrorsStored = _keyValuePersistenceService.GetAsync(_validationContext.ValidationErrorsKey).Result;
            var validationErrorMessagesStored = _keyValuePersistenceService.GetAsync(_validationContext.ValidationErrorMessageLookupKey).Result;

            return _validationErrorCache.ValidationErrors;
        }

        public string SeverityToString(Severity? severity)
        {
            if (severity.HasValue)
            {
                return severity == Severity.Warning ? "W" : "E";
            }

            return null;
        }
    }
}
