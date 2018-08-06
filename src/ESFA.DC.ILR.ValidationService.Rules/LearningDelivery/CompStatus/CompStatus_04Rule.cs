using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_04Rule : AbstractRule, IRule<ILearner>
    {
        public CompStatus_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.OutcomeNullable, learningDelivery.CompStatus))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.CompStatus));
                }
            }
        }

        public bool ConditionMet(int? outcome, int compStatus)
        {
            return !outcome.HasValue && compStatus != 1;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
