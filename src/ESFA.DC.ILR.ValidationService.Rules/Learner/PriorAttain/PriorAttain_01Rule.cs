using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { 10, 25, 82 };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public PriorAttain_01Rule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.PriorAttain_01)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.PriorAttainNullable, learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? priorAttain, int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return PriorAttainConditionMet(priorAttain)
                && FundModelConditionMet(fundModel)
                && LearningDeliveryFAMSConditionMet(fundModel, learningDeliveryFAMs);
        }

        public bool PriorAttainConditionMet(int? priorAttain)
        {
            return !priorAttain.HasValue;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return !_fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryFAMSConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !(fundModel == 99
                && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "108"));
        }
    }
}