using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private const string Fail = "F";
        private readonly IValidationErrors _validationErrors;

        public ValidationErrorsDataRetrievalService(IValidationErrors validationErrors)
        {
            _validationErrors = validationErrors;
        }

        public async Task<IReadOnlyDictionary<string, ValidationError>> RetrieveAsync(CancellationToken cancellationToken)
        {
            return (await _validationErrors
                .Rules
                .ToListAsync(cancellationToken))
                .ToDictionary(
                    ve => ve.Rulename,
                    ve => new ValidationError
                    {
                        Message = ve.Message,
                        RuleName = ve.Rulename,
                        Severity = GetSeverity(ve.Severity)
                    });
        }

        private Severity GetSeverity(string sev)
        {
            switch (sev)
            {
                case Error:
                    return Severity.Error;
                case Warning:
                    return Severity.Warning;
                case Fail:
                    return Severity.Fail;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sev));
            }
        }
    }
}
