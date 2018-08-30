using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OtherFundAdj
{
    public class OtherFundAdj_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { 10, 25, 70, 82 };

        public OtherFundAdj_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OtherFundAdj_01)
        {
        }

        public OtherFundAdj_01Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OtherFundAdjNullable,
                    learningDelivery.FundModel))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.OtherFundAdjNullable));
                    return;
                }
            }
        }

        public bool ConditionMet(int? otherFundAdj, int fundModel)
        {
            return OtherFundAdjConditionMet(otherFundAdj)
                && FundModelConditionMet(fundModel);
        }

        public virtual bool OtherFundAdjConditionMet(int? otherFundAdj)
        {
            return otherFundAdj.HasValue;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? otherFundAdj)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.OtherFundAdj, otherFundAdj)
            };
        }
    }
}
