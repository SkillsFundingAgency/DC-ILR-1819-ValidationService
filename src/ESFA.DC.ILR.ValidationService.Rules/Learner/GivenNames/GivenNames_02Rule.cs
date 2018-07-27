using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames
{
    public class GivenNames_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public GivenNames_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.GivenNames_02)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.GivenNames, objectToValidate.PlanLearnHoursNullable, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.GivenNames, objectToValidate.PlanLearnHoursNullable));
            }
        }

        public bool ConditionMet(string givenNames, int? planLearnHours, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return GivenNamesConditionMet(givenNames)
                && PlanLearnHoursConditionMet(planLearnHours)
                && CrossLearningDeliveryConditionMet(learningDeliveries);
        }

        public bool GivenNamesConditionMet(string givenNames)
        {
            return string.IsNullOrWhiteSpace(givenNames);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours > 10;
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.All(ld => ld.FundModel == 10
                || learningDeliveries.All(ldf => ldf.FundModel == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string givenNames, int? planLearnHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.GivenNames, givenNames),
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours)
            };
        }
    }
}