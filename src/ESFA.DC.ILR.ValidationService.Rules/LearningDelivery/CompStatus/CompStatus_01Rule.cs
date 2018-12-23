using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public CompStatus_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.CompStatus))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.CompStatus));
                }
            }
        }

        public bool ConditionMet(int compStatus)
        {
            return !_provideLookupDetails.Contains(LookupSimpleKey.CompStatus, compStatus);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
