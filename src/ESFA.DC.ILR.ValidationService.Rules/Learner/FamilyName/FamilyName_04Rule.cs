using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName
{
    public class FamilyName_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public FamilyName_04Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (CrossLearningDeliveryConditionMet(objectToValidate.LearningDeliveries) && ConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.ULNNullable, objectToValidate.FamilyName))
            {
                HandleValidationError(RuleNameConstants.FamilyName_04, objectToValidate.LearnRefNumber);
            }
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                && (learningDeliveries.All(ld => ld.FundModelNullable == 10)
                || learningDeliveries.All(ld => ld.FundModelNullable == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool ConditionMet(long? planLearnHours, long? uln, string familyName)
        {
            return planLearnHours.HasValue
                && planLearnHours <= 10
                && uln != ValidationConstants.TemporaryULN
                && string.IsNullOrWhiteSpace(familyName);
        }
    }
}