using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_30Rule : AbstractRule, IRule<ILearner>
    {
        public LearnAimRef_30Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_30)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.AimType))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int aimType)
        {
            return (aimType != 1 && learnAimRef == ValidationConstants.ZPROG001)
                   || (aimType == 1 && learnAimRef != ValidationConstants.ZPROG001);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
