using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3
{
    public class QUALENT3_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2010 = new DateTime(2010, 08, 01);

        public QUALENT3_01Rule(IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.QUALENT3_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, ILearningDeliveryHE learningDeliveryHE)
        {
            return LearningDeliveryHEConditionMet(learningDeliveryHE)
                && LearnStartDateConditionMet(learnStartDate);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null && learningDeliveryHE.QUALENT3 == null;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _augustFirst2010;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
