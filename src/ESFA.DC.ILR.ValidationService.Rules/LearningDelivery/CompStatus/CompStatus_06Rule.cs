using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_06Rule : AbstractRule, IRule<ILearner>
    {
        public CompStatus_06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_06)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.OutcomeNullable, learningDelivery.CompStatus))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.CompStatus, learningDelivery.OutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? outcome, int compStatus)
        {
            return OutcomeConditionMet(outcome) && CompStatusConditionMet(compStatus);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome == 1 || outcome == 8;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus == 3 || compStatus == 6;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus, int? outcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome),
            };
        }
    }
}
