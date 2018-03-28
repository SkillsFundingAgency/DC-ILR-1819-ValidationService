using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model
{
    public struct ValidationError : IValidationError
    {
        public ValidationError(string ruleName, string learnerRefernenceNumber, long? aimSequenceNumber = null,  IEnumerable<string> errorMessageParameters = null)
        {
            LearnerReferenceNumber = learnerRefernenceNumber;
            AimSequenceNumber = aimSequenceNumber;
            RuleName = ruleName;
            ErrorMessageParameters = errorMessageParameters;
        }

        public string LearnerReferenceNumber { get; }

        public long? AimSequenceNumber { get; }

        public string RuleName { get; }

        public IEnumerable<string> ErrorMessageParameters { get; }
    }
}