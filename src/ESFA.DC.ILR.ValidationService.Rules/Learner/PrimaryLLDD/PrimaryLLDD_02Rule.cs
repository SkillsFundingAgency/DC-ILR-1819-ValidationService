using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_02Rule : AbstractRule, IRule<ILearner>
    {
        public PrimaryLLDD_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PrimaryLLDD_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LLDDAndHealthProblems != null)
            {
                foreach (var lldcat in objectToValidate.LLDDAndHealthProblems)
                {
                    if (ConditionMet(lldcat.PrimaryLLDDNullable))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(lldcat.PrimaryLLDDNullable));
                    }
                }
            }
        }

        public bool ConditionMet(int? primaryLLDD)
        {
            return primaryLLDD.HasValue && primaryLLDD.Value != 1;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? primaryLLDD)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PrimaryLLDD, primaryLLDD)
            };
        }
    }
}