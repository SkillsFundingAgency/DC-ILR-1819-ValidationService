using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.AddLine1
{
    public class AddLine1_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public AddLine1_03Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler, RuleNameConstants.Addline1_03)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.AddLine1, objectToValidate.LearningDeliveries, objectToValidate.PlanLearnHoursNullable))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.AddLine1));
            }
        }

        public bool ConditionMet(string addLine1, IEnumerable<ILearningDelivery> learningDeliveries, int? planLearnHours)
        {
            return AddLine1ConditionMet(addLine1)
                   && !(CrossLearningDeliveryConditionMet(learningDeliveries) && PlannedLearnHoursConditionMet(planLearnHours));
        }

        public bool AddLine1ConditionMet(string addLine1)
        {
            return string.IsNullOrWhiteSpace(addLine1);
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.All(ld => ld.FundModel == 10
                                                || learningDeliveries.All(ldf => ldf.FundModel == 99 && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool PlannedLearnHoursConditionMet(int? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours.Value <= 10;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string addLine1)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AddLine1, addLine1),
            };
        }
    }
}