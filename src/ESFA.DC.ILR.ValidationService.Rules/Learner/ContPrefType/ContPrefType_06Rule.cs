using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_06Rule : AbstractRule, IRule<ILearner>
    {
        public ContPrefType_06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_06)
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
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(ContactPreference.Types.RestrictedUserInteraction));
            }
        }

        public bool ConditionMet(IEnumerable<IContactPreference> contactPreferences)
        {
            var filteredContactPreferences = FilterContactPreferencesByPrefType(contactPreferences, ContactPreference.Types.RestrictedUserInteraction);

            if (filteredContactPreferences != null && filteredContactPreferences.Count > 2)
            {
                    return true;
            }

            return false;
        }

        public IList<IContactPreference> FilterContactPreferencesByPrefType(IEnumerable<IContactPreference> contactPreferences, string contPrefType)
        {
            return contactPreferences?.Where(cp => cp.ContPrefType == contPrefType).ToList();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string contPrefType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, contPrefType),
            };
        }
    }
}
