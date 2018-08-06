using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    /// <summary>
    /// LearningDelivery.FundModel = 25 or 82 and Learner.EngGrade is null
    /// </summary>
    public class EngGrade_01Rule : AbstractRule, IRule<ILearner>
    {
        public EngGrade_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(objectToValidate.EngGrade, learningDelivery.FundModelNullable))
                    {
                        HandleValidationError(RuleNameConstants.EngGrade_01Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool ConditionMet(string engGradeNullable, long? fundModelNullable)
        {
            return string.IsNullOrWhiteSpace(engGradeNullable) &&
                    FundModelConditionMet(fundModelNullable);
        }

        public bool FundModelConditionMet(long? fundModelNullable)
        {
            return fundModelNullable.HasValue &&
                   (fundModelNullable.Value == 25 || fundModelNullable.Value == 82);
        }
    }
}