using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE
{
    public class MSTUFEE_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public MSTUFEE_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.MSTUFEE_02)
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
                if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity.MSTUFEE))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity.MSTUFEE));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int mstuFee)
        {
            return !_provideLookupDetails.IsCurrent(LookupTimeRestrictedKey.MSTuFee, mstuFee, learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int mstuFee)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, mstuFee)
            };
        }
    }
}
