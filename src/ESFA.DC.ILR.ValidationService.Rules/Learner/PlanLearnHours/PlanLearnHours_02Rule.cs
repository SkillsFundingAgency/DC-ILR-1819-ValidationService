using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    /// <summary>
    /// If returned, the Planned learning hours should be greater than zero
    /// </summary>
    public class PlanLearnHours_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _fundModels = new HashSet<long> { 25, 82, 35, 36, 81, 10, 99 };

        public PlanLearnHours_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.PlanLearnHoursNullable, learningDelivery.FundModelNullable))
                {
                    HandleValidationError(RuleNameConstants.PlanLearnHours_02Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(long? planLearnHoursNullable, long? fundModel)
        {
            return planLearnHoursNullable.HasValue &&
                   planLearnHoursNullable.Value == 0 &&
                    FundModelConditionMet(fundModel);
        }

        public bool FundModelConditionMet(long? fundModel)
        {
            return fundModel.HasValue && _fundModels.Contains(fundModel.Value);
        }
    }
}