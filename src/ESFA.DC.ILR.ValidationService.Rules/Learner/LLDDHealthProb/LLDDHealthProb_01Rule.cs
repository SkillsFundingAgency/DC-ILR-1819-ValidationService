using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    /// <summary>
    /// The learner's LLDD and health problem must be a valid lookup
    /// </summary>
    public class LLDDHealthProb_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _llDDHealthProbLookupValues = new HashSet<long>() { 1, 2, 9 };

        public LLDDHealthProb_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProbNullable))
            {
                HandleValidationError(RuleNameConstants.LLDDHealthProb_01Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? lldHealthProblem)
        {
            return lldHealthProblem.HasValue &&
                   !_llDDHealthProbLookupValues.Contains(lldHealthProblem.Value);
        }
    }
}