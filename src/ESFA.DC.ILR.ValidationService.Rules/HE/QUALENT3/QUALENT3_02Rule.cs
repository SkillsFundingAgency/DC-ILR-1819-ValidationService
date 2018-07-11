using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3
{
    public class QUALENT3_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2010 = new DateTime(2010, 08, 01);

        private readonly IQUALENT3DataService _iQUALENT3DataService;

        public QUALENT3_02Rule(IQUALENT3DataService iQUALENT3DataService, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.QUALENT3_02)
        {
            _iQUALENT3DataService = iQUALENT3DataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.QUALENT3));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return LearningDeliveryHEConditionMet(learningDeliveryHE)
                && QUALENT3ValidConditionMet(learningDeliveryHE.QUALENT3);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null && learningDeliveryHE.QUALENT3 != null;
        }

        public bool QUALENT3ValidConditionMet(string qualent3)
        {
            return !_iQUALENT3DataService.Exists(qualent3);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string qualent3)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.QUALENT3, qualent3)
            };
        }
    }
}
