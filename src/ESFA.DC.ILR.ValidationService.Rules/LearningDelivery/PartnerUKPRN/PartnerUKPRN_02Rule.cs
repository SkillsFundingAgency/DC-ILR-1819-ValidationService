using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _aimTypeOne = 1;

        public PartnerUKPRN_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PartnerUKPRN_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.PartnerUKPRNNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.AimType, learningDelivery.PartnerUKPRNNullable));
                }
            }
        }

        public bool ConditionMet(int aimType, long? partnerUKPRN)
        {
            return NullConditionMet(partnerUKPRN) && AimTypeConditionMet(aimType);
        }

        public bool NullConditionMet(long? partnerUKPRN)
        {
            return partnerUKPRN.HasValue;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return _aimTypeOne == aimType;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, long? partnerUKPRN)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.PartnerUKPRN, partnerUKPRN)
            };
        }
    }
}
