using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    /// <summary>
    /// Learner.MathGrade = D, DD, DE, E, EE, EF, F, FF, FG, G, GG, N, or U and there is no LearnerFAM record where Learner.LearnFAMType = EDF and Learner.LearnFAMCode = 1
    /// </summary>
    public class MathGrade_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFamQueryService;
        private readonly HashSet<string> _mathGrades = new HashSet<string> { "D", "DD", "DE", "E", "EE", "EF", "F", "FF", "FG", "G", "GG", "N", "U" };

        public MathGrade_03Rule(IValidationErrorHandler validationErrorHandler, ILearnerFAMQueryService learnerFamQueryService)
            : base(validationErrorHandler)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.MathGrade, objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.MathGrade_03Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(string mathGrade, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return !string.IsNullOrWhiteSpace(mathGrade) &&
                   _mathGrades.Contains(mathGrade) &&
                   !_learnerFamQueryService.HasLearnerFAMCodeForType(learnerFams, LearnerFamTypeConstants.EDF, 1);
        }
    }
}