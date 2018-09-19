using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { 35, 70, 81 };
        private readonly HashSet<int> _priorAttains = new HashSet<int> { 97, 98 };

        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public PriorAttain_02Rule(ILARSDataService larsDataService, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.PriorAttain_02)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.PriorAttainNullable, learningDelivery.FundModel, learningDelivery.LearnAimRef, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.PriorAttainNullable));
                    return;
                }
            }
        }

        public bool ConditionMet(int? priorAttain, int fundModel, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return PriorAttainConditionMet(priorAttain)
                && FundModelConditionMet(fundModel)
                && LARSConditionMet(learnAimRef)
                && LearningDeliveryFAMSConditionMet(learningDeliveryFAMs);
        }

        public bool PriorAttainConditionMet(int? priorAttain)
        {
            return priorAttain.HasValue && _priorAttains.Contains((int)priorAttain);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LARSConditionMet(string learnAimRef)
        {
            return _larsDataService.FullLevel2EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1)
                || _larsDataService.FullLevel3EntitlementCategoryMatchForLearnAimRef(learnAimRef, 1);
        }

        public bool LearningDeliveryFAMSConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? priorAttain)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttain)
            };
        }
    }
}
