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
            : base(validationErrorHandler)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (CrossLearningDeliveryConditionMet(objectToValidate.LearningDeliveries) && ConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.GivenNames))
            {
                HandleValidationError(RuleNameConstants.GivenNames_02, objectToValidate.LearnRefNumber);
            }
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                && (learningDeliveries.All(ld => ld.FundModelNullable == 10)
                || learningDeliveries.All(ld => ld.FundModelNullable == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool ConditionMet(long? planLearnHours, string givenNames)
        {
            return planLearnHours.HasValue
                && planLearnHours > 10
                && string.IsNullOrWhiteSpace(givenNames);
        }
    }
}