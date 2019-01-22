using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.ELQ
{
    public class ELQ_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public ELQ_02Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ELQ_02)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                                    objectToValidate.LearnRefNumber,
                                    learningDelivery.AimSeqNumber,
                                    BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.ELQNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE?.ELQNullable != null && !_provideLookupDetails.Contains(LookupSimpleKey.ELQ, learningDeliveryHE.ELQNullable.Value);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? elqNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ELQ, elqNullable)
            };
        }
    }
}
