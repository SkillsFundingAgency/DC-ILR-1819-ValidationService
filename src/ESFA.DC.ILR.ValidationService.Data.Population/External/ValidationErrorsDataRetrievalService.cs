using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ValidationErrorsDataRetrievalService : IValidationErrorsDataRetrievalService
    {
        private const string Error = "E";
        private const string Warning = "W";
        private readonly IValidationErrors _validationErrors;

        public ValidationErrorsDataRetrievalService(IValidationErrors validationErrors)
        {
            _validationErrors = validationErrors;
        }

        public IReadOnlyDictionary<string, ValidationError> Retrieve()
        {
            return _validationErrors
                .Rules
                .AsEnumerable()
                .ToDictionary(
                    ve => ve.Rulename,
                    ve => new ValidationError()
                    {
                        Message = ve.Message,
                        RuleName = ve.Rulename,
                        Severity = ve.Severity == Warning ? Severity.Warning : Severity.Error
                    });
        }
    }
}
