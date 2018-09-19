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

namespace ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE
{
    public class NETFEE_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2012 = new DateTime(2012, 08, 01);

        public NETFEE_01Rule(IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.NETFEE_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

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
            return learningDeliveryHE != null
                && learningDeliveryHE.SSN == null
                && learningDeliveryHE.NETFEENullable == null;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _augustFirst2012;
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
