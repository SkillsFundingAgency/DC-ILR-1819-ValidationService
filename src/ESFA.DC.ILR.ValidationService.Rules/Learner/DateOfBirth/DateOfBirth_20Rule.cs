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
    public class DateOfBirth_20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<long?> _fundModels = new HashSet<long?>() { 25, 82 };

        public DateOfBirth_20Rule(IValidationDataService validationDataService, IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
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
                        _validationDataService.AcademicYearAugustThirtyFirst,
                        _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                            learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "107")))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_20, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool Exclude(long? progType)
        {
            return progType == 24;
        }

        public bool ConditionMet(long? fundModel, DateTime? dateOfBirth, DateTime academicYearAugustThirtyFirst, bool hasSOF107)
        {
            return !hasSOF107
                && fundModel.HasValue
                && _fundModels.Contains(fundModel.Value)
                && dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearAugustThirtyFirst) < 19;
        }
    }
}