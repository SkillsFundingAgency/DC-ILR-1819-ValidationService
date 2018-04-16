using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_04Rule : AbstractRule, IRule<ILearner>
    {
        private const double HoursInDay = 24;

        private readonly ILearningDeliveryQueryService _learningDeliveryQueryService;

        public AddHours_04Rule(ILearningDeliveryQueryService learningDeliveryQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AddHours_04)
        {
            _learningDeliveryQueryService = learningDeliveryQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (AddHoursConditionMet(learningDelivery.AddHoursNullable))
                {
                    if (ConditionMet(_learningDeliveryQueryService.AverageAddHoursPerLearningDay(learningDelivery)))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                    }
                }
            }
        }

        public bool AddHoursConditionMet(int? addHours)
        {
            return addHours > 60;
        }

        public bool ConditionMet(double? averageHoursPerLearningDay)
        {
            return averageHoursPerLearningDay > HoursInDay;
        }
    }
}
