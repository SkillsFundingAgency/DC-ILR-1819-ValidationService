using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    /// <summary>
    /// If returned, the sum of the Planned learning hours and the Planned employability, enrichment and pastoral hours must be greater than zero
    /// </summary>
    public class PlanLearnHours_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _fundModels = new HashSet<long> { 25, 82 };

        public PlanLearnHours_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.PlanEEPHoursNullable, learningDelivery.FundModelNullable))
                {
                    HandleValidationError(RuleNameConstants.PlanLearnHours_03Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(long? planLearnHours, long? planEeepHours, long? fundModel)
        {
            return planLearnHours.HasValue &&
                   planLearnHours.Value + (planEeepHours ?? 0) == 0
                   && FundModelConditionMet(fundModel);
        }

        public bool FundModelConditionMet(long? fundModel)
        {
            return fundModel.HasValue && _fundModels.Contains(fundModel.Value);
        }
    }
}