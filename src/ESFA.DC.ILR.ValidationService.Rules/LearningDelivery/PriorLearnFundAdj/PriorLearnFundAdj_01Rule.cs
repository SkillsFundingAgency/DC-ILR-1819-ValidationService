using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj
{
    public class PriorLearnFundAdj_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { 10, 25, 70, 82, 99 };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public PriorLearnFundAdj_01Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PriorLearnFundAdj_01)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public PriorLearnFundAdj_01Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.PriorLearnFundAdjNullable,
                    learningDelivery.FundModel,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.PriorLearnFundAdjNullable, LearningDeliveryFAMTypeConstants.ADL));
                }
            }
        }

        public bool ConditionMet(int? priorLearnFundAdj, int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return PriorLearnFundAdjConditionMet(priorLearnFundAdj)
                && FundModelConditionMet(fundModel)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public virtual bool PriorLearnFundAdjConditionMet(int? priorLearnFundAdj)
        {
            return priorLearnFundAdj.HasValue;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? priorLearnFundAdj, string famType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.PriorLearnFundAdj, priorLearnFundAdj),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType)
            };
        }
    }
}
