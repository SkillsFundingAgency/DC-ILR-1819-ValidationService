using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_05Rule : AbstractRule, IRule<ILearner>
    {
        public AimType_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel))
                {
                    HandleValidationError(RuleNameConstants.AimType_05, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel)
        {
            return aimType == 5 && fundModel != FundModelConstants.CommunityLearning && fundModel != FundModelConstants.SixteenToNineteen;
        }
    }
}
