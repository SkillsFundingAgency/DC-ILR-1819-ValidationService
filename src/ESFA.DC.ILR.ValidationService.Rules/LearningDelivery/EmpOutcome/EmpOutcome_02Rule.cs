using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public EmpOutcome_02Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpOutcome_02)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.EmpOutcomeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.EmpOutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? empOutcome)
        {
            return empOutcome.HasValue && !_provideLookupDetails.Contains(TypeOfLimitedLifeLookup.EmpOutcome, empOutcome.Value);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? empOutcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpOutcome, empOutcome)
            };
        }
    }
}
