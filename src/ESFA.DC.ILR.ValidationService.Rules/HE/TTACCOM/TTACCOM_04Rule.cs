using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    public class TTACCOM_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2013 = new DateTime(2013, 08, 01);
        private readonly IDD06 _dd06;

        public TTACCOM_04Rule(IDD06 dd06, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.TTACCOM_04)
        {
            _dd06 = dd06;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null
                && LearnStartDateConditionMet(objectToValidate.LearningDeliveries)
                && LearnerHEConditionMet(objectToValidate.LearnerHEEntity))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.MODESTUD));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return LearningDeliveryHEConditionMet(learningDelivery);
        }

        public bool LearnerHEConditionMet(ILearnerHE learnerHE)
        {
            return learnerHE != null && !learnerHE.TTACCOMNullable.HasValue;
        }

        public bool LearningDeliveryHEConditionMet(ILearningDelivery learningDelivery)
        {
            return learningDelivery != null
                && learningDelivery.LearningDeliveryHEEntity != null
                && learningDelivery.LearningDeliveryHEEntity.MODESTUD == 1;
        }

        public bool LearnStartDateConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return _dd06.Derive(learningDeliveries) >= _augustFirst2013;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int modeSTUD)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, modeSTUD)
            };
        }
    }
}
