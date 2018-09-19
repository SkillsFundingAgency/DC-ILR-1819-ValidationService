using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    /// <summary>
    /// DD06 > 'Valid to' in ILR_ContPrefTypeCode
    /// </summary>
    public class ContPrefType_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD06 _dd06;
        private readonly IContactPreferenceInternalDataService _contactPreferenceDataService;

        public ContPrefType_03Rule(IValidationErrorHandler validationErrorHandler, IContactPreferenceInternalDataService contactPreferenceDataService, IDD06 dd06)
            : base(validationErrorHandler)
        {
            _dd06 = dd06;
            _contactPreferenceDataService = contactPreferenceDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.ContactPreferences != null)
            {
                foreach (var contactPreference in objectToValidate.ContactPreferences)
                {
                    if (ConditionMet(
                        contactPreference.ContPrefType,
                        contactPreference.ContPrefCodeNullable,
                        _dd06.Derive(objectToValidate.LearningDeliveries)))
                    {
                        HandleValidationError(RuleNameConstants.ContPrefType_03Rule, objectToValidate.LearnRefNumber);
                    }
                }
            }
        }

        public bool ConditionMet(string contactPreferenceType, long? contPrefCode, DateTime? minimumStartDate)
        {
            return !string.IsNullOrWhiteSpace(contactPreferenceType) &&
                   contPrefCode.HasValue &&
                   minimumStartDate.HasValue &&
                   _contactPreferenceDataService.TypeExists(contactPreferenceType) &&
                   !_contactPreferenceDataService.TypeForCodeExist(contactPreferenceType, contPrefCode.Value, minimumStartDate.Value);
        }
    }
}