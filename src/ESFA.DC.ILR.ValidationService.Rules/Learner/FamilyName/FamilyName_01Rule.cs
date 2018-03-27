using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName
{
    public class FamilyName_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public FamilyName_01Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null || !objectToValidate.LearningDeliveries.All(ld => Exclude(ld)))
            {
                if (ConditionMet(objectToValidate.FamilyName))
                {
                    HandleValidationError(RuleNameConstants.FamilyName_01, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(string familyName)
        {
            return string.IsNullOrWhiteSpace(familyName);
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModelNullable == 10
                || (learningDelivery.FundModelNullable == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108"));
        }
    }
}