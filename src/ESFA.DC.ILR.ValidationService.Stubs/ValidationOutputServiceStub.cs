using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ValidationOutputServiceStub : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorCache _validationErrorCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly IKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IValidationContext _validationContext;
        private readonly ISerializationService _serializationService;

        public ValidationOutputServiceStub(IValidationErrorCache validationErrorCache, ICache<IMessage> messageCache, IKeyValuePersistenceService keyValuePersistenceService, IValidationContext validationContext, ISerializationService serializationService)
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

            _keyValuePersistenceService.SaveAsync(_validationContext.ValidLearnRefNumbersKey, _serializationService.Serialize(validLearnerRefNumbers)).Wait();
            _keyValuePersistenceService.SaveAsync(_validationContext.InvalidLearnRefNumbersKey, _serializationService.Serialize(invalidLearnerRefNumbers));

            var valid = _keyValuePersistenceService.GetAsync(_validationContext.ValidLearnRefNumbersKey).Result;
            var invalid = _keyValuePersistenceService.GetAsync(_validationContext.InvalidLearnRefNumbersKey).Result;
                
            return _validationErrorCache.ValidationErrors;
        }
    }
}
