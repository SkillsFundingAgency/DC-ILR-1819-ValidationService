using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_04Rule : AbstractRule, IRule<ILearner>
    {
        public LLDDHealthProb_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler,  RuleNameConstants.LLDDHealthProb_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProb, objectToValidate.LLDDAndHealthProblems))
            {
                HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.LLDDHealthProb));
            }
        }

        public bool ConditionMet(int lldHealthProblem, IReadOnlyCollection<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return lldHealthProblem == LLDDHealthProblemConstants.NoLearningDifficulty &&
                   (llddAndHealthProblems != null && llddAndHealthProblems.Any());
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int lldHealthProblem)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LLDDHealthProb, lldHealthProblem)
            };
        }
    }
}