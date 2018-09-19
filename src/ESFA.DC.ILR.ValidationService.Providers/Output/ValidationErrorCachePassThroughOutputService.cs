using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<IValidationError>> ProcessAsync(CancellationToken cancellationToken)
        {
            return _validationErrorCache.ValidationErrors;
        }
    }
}
