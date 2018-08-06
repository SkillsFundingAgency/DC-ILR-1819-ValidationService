using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryQueryService _learningDeliveryQueryService;

        public AddHours_06Rule(ILearningDeliveryQueryService learningDeliveryQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AddHours_06)
        {
            _learningDeliveryQueryService = learningDeliveryQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(_learningDeliveryQueryService.AverageAddHoursPerLearningDay(learningDelivery)))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearnPlanEndDate, learningDelivery.AddHoursNullable));
                }
            }
        }

        public bool ConditionMet(double? addHoursPerDay)
        {
            return addHoursPerDay > 9 && addHoursPerDay < 24;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime? learnPlanEndDate, int? addHoursNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AddHours, addHoursNullable)
            };
        }
    }
}
