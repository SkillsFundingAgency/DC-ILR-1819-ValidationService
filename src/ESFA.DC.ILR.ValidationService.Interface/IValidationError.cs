using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationError
    {
        string LearnerReferenceNumber { get; }

        long? AimSequenceNumber { get; }

        string RuleName { get; }

        IEnumerable<string> ErrorMessageParameters { get; }
    }
}
