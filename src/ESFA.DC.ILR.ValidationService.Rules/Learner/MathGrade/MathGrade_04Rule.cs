using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    /// <summary>
    /// Learner.LearnFAMType = MCF and Learner.LearnFAMCode = 2, 3 or 4 and Learner.MathGrade <> "NONE"
    /// </summary>
    public class MathGrade_04Rule : AbstractRule, IRule<ILearner>
    {
        private const string MathGradeNone = "NONE";
        private readonly ILearnerFAMQueryService _learnerFamQueryService;
        private readonly HashSet<long> _famCodes = new HashSet<long>() { 2, 3, 4 };

        public MathGrade_04Rule(IValidationErrorHandler validationErrorHandler, ILearnerFAMQueryService learnerFamQueryService)
            : base(validationErrorHandler)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.MathGrade, objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.MathGrade_04Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(string mathGrade, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return !string.IsNullOrWhiteSpace(mathGrade) &&
                   mathGrade != MathGradeNone &&
                   _learnerFamQueryService.HasAnyLearnerFAMCodesForType(learnerFams, LearnerFamTypeConstants.MCF, _famCodes);
        }
    }
}