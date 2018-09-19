using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    /// <summary>
    /// If the learner's learning aim is EFA funded, the GCSE maths qualification grade must be returned
    /// </summary>
    public class MathGrade_01Rule : AbstractRule, IRule<ILearner>
    {
        public MathGrade_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.MathGrade, learningDelivery.FundModelNullable))
                {
                    HandleValidationError(RuleNameConstants.MathGrade_01Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(string mathGradeNullable, long? fundModelNullable)
        {
            return string.IsNullOrWhiteSpace(mathGradeNullable) &&
                    FundModelConditionMet(fundModelNullable);
        }

        public bool FundModelConditionMet(long? fundModelNullable)
        {
            return fundModelNullable.HasValue &&
                   (fundModelNullable.Value == 25 || fundModelNullable.Value == 82);
        }
    }
}