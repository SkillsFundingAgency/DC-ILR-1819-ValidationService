using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_04Rule : AbstractRule, IRule<ILearner>
    {
        public WithdrawReason_04Rule(
            IValidationErrorHandler validationErrorHandler)
        : base(validationErrorHandler, RuleNameConstants.WithdrawReason_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.CompStatus,
                    learningDelivery.WithdrawReasonNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.CompStatus, learningDelivery.WithdrawReasonNullable));
                }
            }
        }

        public bool ConditionMet(int compStatus, int? withdrawReason)
        {
            return CompStatusConditionMet(compStatus)
                   && WithdrawReasonConditionMet(withdrawReason);
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            var compStatuses = new int[] { 1, 2, 6 };

            return compStatuses.Contains(compStatus);
        }

        public bool WithdrawReasonConditionMet(int? withdrawReason)
        {
            return !withdrawReason.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus, int? withdrawReason)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.WithdrawReason, withdrawReason),
            };
        }
    }
}
