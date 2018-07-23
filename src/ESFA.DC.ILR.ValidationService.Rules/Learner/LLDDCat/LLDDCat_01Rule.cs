using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat
{
    public class LLDDCat_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILLDDCatDataService _llddCatDataService;

        public LLDDCat_01Rule(ILLDDCatDataService llddCatDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDCat_01)
        {
            _llddCatDataService = llddCatDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LLDDAndHealthProblems != null)
            {
                foreach (var llddAndHealthProblem in objectToValidate.LLDDAndHealthProblems)
                {
                    if (ConditionMet(llddAndHealthProblem.LLDDCat))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(llddAndHealthProblem.LLDDCat));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int llddCat)
        {
            return !_llddCatDataService.Exists(llddCat);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int llddCat)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LLDDCat, llddCat),
            };
        }
    }
}