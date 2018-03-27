using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    // <summary>
    // The Prior attainment code must be returned
    // Exclusion : This rule is not triggered by community learning aims (LearningDelivery.FundModel = 10) or
    // (LearningDelivery.FundModel = 99 and  (LearningDeliveryFAM.LearnDelFAMType = SOF and LearningDeliveryFAM.LearnDelFAMCode = 108)).
    // This rule is also not triggered by EFA funded learners (LearningDelivery.FundModel = 25 or 82)
    // </summary>
    public class PriorAttain_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly HashSet<long> _excludeFundModels = new HashSet<long> { 10, 25, 82 };

        public PriorAttain_01Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
           : base(validationErrorHandler)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld => !Exclude(ld)))
            {
                if (ConditionMet(objectToValidate.PriorAttainNullable))
                {
                    HandleValidationError(RuleNameConstants.PriorAttain_01Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(long? priorAttain)
        {
            return !priorAttain.HasValue;
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            var fundModelConditionMet = learningDelivery.FundModelNullable.HasValue
                && _excludeFundModels.Contains(learningDelivery.FundModelNullable.Value);

            var famTypeConditionMet = learningDelivery.FundModelNullable.HasValue &&
                            (learningDelivery.FundModelNullable.Value == 99
                            && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(
                            learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108"));

            return fundModelConditionMet || famTypeConditionMet;
        }
    }
}