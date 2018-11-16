using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int?[] validWithdrawReasons = { 2, 3, 7, 28, 29, 40, 41, 42, 43, 44, 45, 46, 47, 97, 98 };

        public WithdrawReason_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.WithdrawReasonNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.WithdrawReasonNullable));
                }
            }
        }

        public bool ConditionMet(int? withdrawReason)
        {
            return withdrawReason.HasValue
                   && !validWithdrawReasons.Contains(withdrawReason);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? withdrawReason)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WithdrawReason, withdrawReason)
            };
        }
    }
}
