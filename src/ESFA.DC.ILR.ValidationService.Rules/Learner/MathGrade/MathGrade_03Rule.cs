using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    public class MathGrade_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFamQueryService;
        private readonly HashSet<string> _mathGrades = new HashSet<string> { "D", "DD", "DE", "E", "EE", "EF", "F", "FF", "FG", "G", "GG", "N", "U" };

        public MathGrade_03Rule(IValidationErrorHandler validationErrorHandler, ILearnerFAMQueryService learnerFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.MathGrade_03)
        {
            _learnerFamQueryService = learnerFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.MathGrade, objectToValidate.LearnerFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, null, BuildErrorMessageParameters(objectToValidate.MathGrade, LearnerFAMTypeConstants.EDF, 1));
            }
        }

        public bool ConditionMet(string mathGrade, IEnumerable<ILearnerFAM> learnerFams)
        {
            return LearnerMathGradeConditionMet(mathGrade)
                && LearnerFAMsConditionMet(learnerFams);
        }

        public bool LearnerMathGradeConditionMet(string mathGrade)
        {
            return !string.IsNullOrWhiteSpace(mathGrade)
                && _mathGrades.Contains(mathGrade);
        }

        public bool LearnerFAMsConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return !_learnerFamQueryService.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EDF, 1);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string mathGrade, string learnFAMType, int learnFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MathGrade, mathGrade),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, learnFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, learnFAMCode)
            };
        }
    }
}