using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat
{
    public class LLDDCat_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD06 _dd06;
        private readonly IProvideLookupDetails _provideLookupDetails;

        public LLDDCat_02Rule(IDD06 dd06, IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDCat_02)
        {
            _dd06 = dd06;
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LLDDAndHealthProblems != null)
            {
                foreach (var lldcat in objectToValidate.LLDDAndHealthProblems)
                {
                    if (ConditionMet(lldcat.LLDDCat, objectToValidate.LearningDeliveries))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber);
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int llddCat, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return _provideLookupDetails.Contains(LookupTimeRestrictedKey.LLDDCat, llddCat)
                && !_provideLookupDetails.IsCurrent(LookupTimeRestrictedKey.LLDDCat, llddCat, _dd06.Derive(learningDeliveries));
        }
    }
}