using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat
{
    /// <summary>
    /// LLDDandHealthProblem.LLDDCat <> null and LLDDandHealthProblem.LLDDCat <> valid lookup on ILR_LLDDCat
    /// </summary>
    public class LLDDCat_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILlddCatInternalDataService _llddCatDataService;

        public LLDDCat_01Rule(IValidationErrorHandler validationErrorHandler, ILlddCatInternalDataService llddCatDataService)
            : base(validationErrorHandler)
        {
            _llddCatDataService = llddCatDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var lldcat in objectToValidate.LLDDAndHealthProblems)
            {
                if (ConditionMet(lldcat.LLDDCatNullable))
                {
                    HandleValidationError(RuleNameConstants.LLDDCat_01Rule, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(long? llddCategory)
        {
            return llddCategory.HasValue &&
                   !_llddCatDataService.CategoryExists(llddCategory);
        }
    }
}