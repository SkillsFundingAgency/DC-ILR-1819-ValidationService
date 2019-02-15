using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat
{
    public class LLDDCat_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public LLDDCat_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDCat_01)
        {
            _provideLookupDetails = provideLookupDetails;
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
            return !_provideLookupDetails.Contains(TypeOfLimitedLifeLookup.LLDDCat, llddCat);
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