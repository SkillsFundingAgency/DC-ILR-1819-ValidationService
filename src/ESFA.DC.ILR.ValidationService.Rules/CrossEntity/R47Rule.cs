using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R47Rule : AbstractRule, IRule<ILearner>
    {
        public R47Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R47)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.ContactPreferences == null)
            {
                return;
            }

            var duplicates = objectToValidate.ContactPreferences
                .GroupBy(cp => new { cp.ContPrefType, cp.ContPrefCode })
                .Where(grp => grp.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    null,
                    BuildErrorMessageParameters(duplicate.Key.ContPrefType, duplicate.Key.ContPrefCode));
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string contPrefType, int contPrefCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, contPrefType),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefCode, contPrefCode)
            };
        }
    }
}