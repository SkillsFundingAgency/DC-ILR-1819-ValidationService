using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidationErrorCachePassThroughOutputService : IValidationOutputService<IValidationError>
    {
        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;

        public ValidationErrorCachePassThroughOutputService(IValidationErrorCache<IValidationError> validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public IEnumerable<IValidationError> Process()
        {
            return _validationErrorCache.ValidationErrors;
        }
    }
}
