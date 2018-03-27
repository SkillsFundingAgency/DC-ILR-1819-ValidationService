using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_13Rule(IValidationDataService validationDataService, IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _validationDataService = validationDataService;
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.FundModelNullable,
                        objectToValidate.DateOfBirthNullable,
                        _validationDataService.AcademicYearEnd,
                        _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                            learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "1")))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_13, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool ConditionMet(long? fundModel, DateTime? dateOfBirth, DateTime academicYearEnd, bool hasSOFOne)
        {
            return hasSOFOne
                && fundModel.HasValue
                && fundModel.Value == 99
                && dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearEnd) < 16;
        }
    }
}