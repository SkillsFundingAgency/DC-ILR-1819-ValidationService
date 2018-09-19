using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_03Rule : AbstractRule, IRule<ILearner>
    {
        public PrimaryLLDD_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PrimaryLLDD_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDAndHealthProblems))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems)
        {
            return lLDDAndHealthProblems != null &&
                   lLDDAndHealthProblems.Count(x =>
                       x.PrimaryLLDDNullable.HasValue && x.PrimaryLLDDNullable.Value == 1) > 1;
        }
    }
}