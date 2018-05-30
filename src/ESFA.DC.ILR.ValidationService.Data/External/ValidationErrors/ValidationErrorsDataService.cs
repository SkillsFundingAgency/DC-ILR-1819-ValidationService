using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors
{
    public class ValidationErrorsDataService : IValidationErrorsDataService
    {
        private readonly Severity? _nullSeverity = null;
        private readonly IExternalDataCache _externalDataCache;

        public ValidationErrorsDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public Severity? SeverityForRuleName(string ruleName)
        {
            _externalDataCache.ValidationErrors.TryGetValue(ruleName, out var error);

            return error?.Severity ?? _nullSeverity;
        }
    }
}
