using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    /// <summary>
    /// If there is only one LLDD and Health problem record then the Primary LLDD and health problem must be recorded
    /// </summary>
    public class PrimaryLLDD_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _excludeLlddCatValues = new HashSet<long>() { 98, 99 };

        public PrimaryLLDD_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDAndHealthProblems) && !Exclude(objectToValidate.LLDDAndHealthProblems))
            {
                HandleValidationError(RuleNameConstants.PrimaryLLDD_04Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems)
        {
            return lLDDAndHealthProblems != null &&
                   lLDDAndHealthProblems.Count == 1 &&
                   !lLDDAndHealthProblems.Single().PrimaryLLDDNullable.HasValue;
        }

        public bool Exclude(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems)
        {
            return lLDDAndHealthProblems != null &&
                   lLDDAndHealthProblems.All(x => x.LLDDCatNullable.HasValue && _excludeLlddCatValues.Contains(x.LLDDCatNullable.Value));
        }
    }
}