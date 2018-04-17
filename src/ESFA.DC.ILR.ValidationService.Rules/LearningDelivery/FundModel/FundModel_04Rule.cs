using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;

        public FundModel_04Rule(IDD07 dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_04)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable))
                {
                }
            }
        }

        public bool ConditionMet(int? progType)
        {
            return false;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModelConstants.CommunityLearning || fundModel == FundModelConstants.SixteenToNineteen;
        }

        public bool ApprenticeshipConditionMet(int? progType)
        {
            return _dd07.Derive(progType) == ValidationConstants.Y;
        }
    }
}
