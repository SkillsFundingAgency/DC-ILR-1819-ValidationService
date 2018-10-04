using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R70Rule : AbstractRule, IRule<ILearner>
    {
        private int _progType = 25;
        private int _compAimType = 3;
        private int _progAimType = 1;

        public R70Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R70)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveryStandardCompAim =
                objectToValidate.LearningDeliveries
                .Where(ld => ld.ProgTypeNullable == _progType && ld.AimType == _compAimType).FirstOrDefault();

            if (ConditionMet(learningDeliveryStandardCompAim, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    learningDeliveryStandardCompAim.AimSeqNumber,
                    BuildErrorMessageParameters(
                        learningDeliveryStandardCompAim.AimType,
                        learningDeliveryStandardCompAim.FundModel,
                        learningDeliveryStandardCompAim.ProgTypeNullable,
                        learningDeliveryStandardCompAim.StdCodeNullable));
            }
        }

        public bool ConditionMet(ILearningDelivery learningDeliveryStandardCompAim, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return LearningDeliveryStandardCompAimConditionMet(learningDeliveryStandardCompAim)
                && LearningDeliveryStandardProgAimConditionMet(learningDeliveryStandardCompAim, learningDeliveries);
        }

        public bool LearningDeliveryStandardCompAimConditionMet(ILearningDelivery learningDeliveryStandardCompAim)
        {
            return learningDeliveryStandardCompAim != null;
        }

        public bool LearningDeliveryStandardProgAimConditionMet(ILearningDelivery learningDeliveryStandardCompAim, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return
                !learningDeliveries
                .Any(ld =>
                   ld.ProgTypeNullable == learningDeliveryStandardCompAim.ProgTypeNullable
                && ld.StdCodeNullable == learningDeliveryStandardCompAim.StdCodeNullable
                && ld.AimType == _progAimType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType, int? stdCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode)
            };
        }
    }
}
