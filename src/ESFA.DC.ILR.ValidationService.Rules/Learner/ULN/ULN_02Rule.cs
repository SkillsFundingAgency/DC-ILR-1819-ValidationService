using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<int> _fundModels = new int[] { 99, 10 };

        public ULN_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_02)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, objectToValidate.ULN, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
                    return;
                }
            }
        }

        public bool ConditionMet(int fundModel, long uln, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ULNConditionMet(fundModel, uln)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool ULNConditionMet(int fundModel, long uln)
        {
            return _fundModels.Contains(fundModel) && uln == ValidationConstants.TemporaryULN;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "1");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln)
            };
        }
    }
}