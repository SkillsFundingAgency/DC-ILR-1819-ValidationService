using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorCache : IValidationErrorCache<IValidationError>
    {
        private readonly ConcurrentBag<IValidationError> _validationErrors = new ConcurrentBag<IValidationError>();

        public IReadOnlyCollection<IValidationError> ValidationErrors
        {
            get { return _validationErrors; }
        }

        public void Add(IValidationError validationError)
        {
            _validationErrors.Add(validationError);
        }
    }
}
