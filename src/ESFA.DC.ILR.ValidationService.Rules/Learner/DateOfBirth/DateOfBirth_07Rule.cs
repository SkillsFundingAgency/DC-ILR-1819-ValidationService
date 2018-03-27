using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<long> _fundModels = new HashSet<long>() { 25, 82 };

        public DateOfBirth_07Rule(IValidationDataService validationDataService, IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _validationDataService = validationDataService;
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (LearnerConditionMet(objectToValidate.DateOfBirthNullable, _validationDataService.AcademicYearAugustThirtyFirst))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (LearningDeliveryConditionMet(learningDelivery.FundModelNullable, _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "107")))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_07, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool LearnerConditionMet(DateTime? dateOfBirth, DateTime academicYearAugustThirtyFirst)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearAugustThirtyFirst) >= 25;
        }

        public bool LearningDeliveryConditionMet(long? fundModel, bool hasSOFCode107)
        {
            return hasSOFCode107
                && fundModel.HasValue
                && _fundModels.Contains(fundModel.Value);
        }
    }
}