using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _contPrefCodesSet1 = { 1, 2, 3 };
        private readonly int[] _contPrefCodesSet2 = { 4, 5, 6 };

        public ContPrefType_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.ContactPreferences == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.ContactPreferences))
            {
                var contPrefCodes = objectToValidate.ContactPreferences
                    .Where(cp => cp.ContPrefType.CaseInsensitiveEquals(ContactPreference.Types.PreferredMethodOfContact))
                    .Select(cp => cp.ContPrefCode);

                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(
                        ContactPreference.Types.PreferredMethodOfContact,
                        string.Join(", ", contPrefCodes)));
            }
        }

        public bool ConditionMet(IEnumerable<IContactPreference> contactPreferences)
        {
            return HasAnyPMUContactPreferenceForCodes(contactPreferences, _contPrefCodesSet1)
                && HasAnyPMUContactPreferenceForCodes(contactPreferences, _contPrefCodesSet2);
        }

        public bool HasAnyPMUContactPreferenceForCodes(IEnumerable<IContactPreference> contactPreferences, IEnumerable<int> contPrefCodes)
        {
            return contactPreferences
                .Any(cp =>
                    cp.ContPrefType.CaseInsensitiveEquals(ContactPreference.Types.PreferredMethodOfContact) &&
                    contPrefCodes.Contains(cp.ContPrefCode));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string contPrefType, string contPrefCodes)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefCode, contPrefCodes),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, contPrefType)
            };
        }
    }
}
