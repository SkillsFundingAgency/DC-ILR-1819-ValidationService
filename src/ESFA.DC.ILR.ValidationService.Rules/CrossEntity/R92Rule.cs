using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R92Rule : AbstractRule, IRule<ILearner>
    {
        private const string _learnAimRef = ValidationConstants.ZESF0001;

        public R92Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R92)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var uniqueConRefNumbersForError = DuplicateConRefNumbersForLearnAimRef(objectToValidate.LearningDeliveries);

            if (!string.IsNullOrWhiteSpace(uniqueConRefNumbersForError))
            {
                HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            errorMessageParameters: BuildErrorMessageParameters(
                            _learnAimRef,
                            uniqueConRefNumbersForError));
            }
        }

        public string DuplicateConRefNumbersForLearnAimRef(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var conRefNumbers = learningDeliveries?
                .Where(ld => ld.LearnAimRef.CaseInsensitiveEquals(_learnAimRef))
                .GroupBy(ld => ld.ConRefNumber, StringComparer.OrdinalIgnoreCase)
                .Where(c => c.Count() > 1)
                .Select(c => c.Key);

            return conRefNumbers != null
                ? string.Join(", ", conRefNumbers)
                : string.Empty;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber)
            };
        }
    }
}
