using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    /// <summary>
    /// .LLDDandHealthProblem.PrimaryLLDD<> null and LLDDandHealthProblem.PrimaryLLDD <> valid look up on ILR_PrimaryLLDD
    /// </summary>
    public class PrimaryLLDD_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _validPrimaryLldd = 1;

        public PrimaryLLDD_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var lldcat in objectToValidate.LLDDAndHealthProblems)
            {
                if (ConditionMet(lldcat.PrimaryLLDDNullable))
                {
                    HandleValidationError(RuleNameConstants.PrimaryLLDD_02Rule, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(long? primaryLlddValue)
        {
            return primaryLlddValue.HasValue && primaryLlddValue.Value != _validPrimaryLldd;
        }
    }
}