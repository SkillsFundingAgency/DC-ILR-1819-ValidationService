using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.FUNDCOMP
{
    public class FUNDCOMP_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public FUNDCOMP_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FUNDCOMP_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(x => x.LearningDeliveryHEEntity != null))
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity.FUNDCOMP))
                {
                    HandleValidationError(
                                    objectToValidate.LearnRefNumber,
                                    learningDelivery.AimSeqNumber,
                                    BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.FUNDCOMP));
                }
            }
        }

        public bool ConditionMet(int fundComp)
        {
            return !_provideLookupDetails.Contains(TypeOfLimitedLifeLookup.FundComp, fundComp);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundComp)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FUNDCOMP, fundComp)
            };
        }
    }
}
