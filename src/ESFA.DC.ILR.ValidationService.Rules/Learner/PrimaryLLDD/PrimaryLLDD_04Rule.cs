using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _excludeLlddCatValues = new HashSet<long>() { 98, 99 };

        public PrimaryLLDD_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PrimaryLLDD_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProb, objectToValidate.LLDDAndHealthProblems))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(int llddHealthProb, IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return LLDDHealthProbConditionMet(llddHealthProb)
                && LLDDConditionMet(llddAndHealthProblems);
        }

        public bool LLDDHealthProbConditionMet(int llddHealthProb)
        {
            return llddHealthProb == 1;
        }

        public bool LLDDConditionMet(IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return llddAndHealthProblems != null
                && llddAndHealthProblems.Count() == 1
                && !_excludeLlddCatValues.Contains(llddAndHealthProblems.First().LLDDCat)
                && !llddAndHealthProblems.First().PrimaryLLDDNullable.HasValue;
        }
    }
}