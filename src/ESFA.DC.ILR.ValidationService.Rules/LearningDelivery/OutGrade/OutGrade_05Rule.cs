using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string[] _validOutGrades = { Grades.FL, Grades.U, Grades.N, Grades.X, Grades.Y };

        public OutGrade_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_05)
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
                if (ConditionMet(
                    learningDelivery.OutcomeNullable,
                    learningDelivery.OutGrade))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OutcomeNullable, learningDelivery.OutGrade));
                }
            }
        }

        public bool ConditionMet(int? outcome, string outGrade)
        {
            return OutcomeConditionMet(outcome)
                   && OutGradeConditionMet(outGrade);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome == Constants.OutcomeConstants.Achieved;
        }

        public bool OutGradeConditionMet(string outGrade)
        {
            return !string.IsNullOrWhiteSpace(outGrade) && _validOutGrades.Any(x => x.CaseInsensitiveEquals(outGrade));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? outcome, string outGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome),
                BuildErrorMessageParameter(PropertyNameConstants.OutGrade, outGrade)
            };
        }
    }
}
