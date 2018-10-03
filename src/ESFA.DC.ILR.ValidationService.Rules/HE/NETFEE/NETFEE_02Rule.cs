using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE
{
    public class NETFEE_02Rule : AbstractRule, IRule<ILearner>
    {
        public NETFEE_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.NETFEE_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.NETFEENullable));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return LearningDeliveryHEConditionMet(learningDeliveryHE);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null
                && learningDeliveryHE.NETFEENullable.HasValue
                && learningDeliveryHE.NETFEENullable > 9000;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? netFee)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.NETFEE, netFee)
            };
        }
    }
}
