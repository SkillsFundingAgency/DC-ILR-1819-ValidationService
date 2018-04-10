using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimSeqNumber
{
    public class AimSeqNumber_02Rule : AbstractRule, IRule<ILearner>
    {
        public AimSeqNumber_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var aimCount = objectToValidate.LearningDeliveries.Count;

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(aimCount, learningDelivery.AimSeqNumber))
                {
                    HandleValidationError(RuleNameConstants.AimSeqNumber_02, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int aimCount, int aimSequenceNumber)
        {
            return aimSequenceNumber > aimCount;
        }
    }
}
