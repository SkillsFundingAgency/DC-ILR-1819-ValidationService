using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanEEPHours
{
    /// <summary>
    /// If the learner's learning aim is EFA funded, the Planned employability, enrichment and pastoral hours, must be returned
    /// </summary>
    public class PlanEEPHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;

        public PlanEEPHours_01Rule(IDD07 dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (!HasAllLearningAimsClosedExcludeConditionMet(objectToValidate.LearningDeliveries))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld => !Exclude(ld)))
                {
                    if (ConditionMet(objectToValidate.PlanEEPHoursNullable, learningDelivery.FundModelNullable))
                    {
                        HandleValidationError(RuleNameConstants.PlanEEPHours_01Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool ConditionMet(long? planEepHoursNullable, long? fundModelNullable)
        {
            return !planEepHoursNullable.HasValue &&
                    FundModelConditionMet(fundModelNullable);
        }

        public bool FundModelConditionMet(long? fundModelNullable)
        {
            return fundModelNullable.HasValue &&
                   (fundModelNullable.Value == 25 || fundModelNullable.Value == 82);
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            return HasLearningDeliveryDd07ExcludeConditionMet(_dd07.Derive(learningDelivery.ProgTypeNullable)) ||
                   HasLearningDeliveryFundModelExcludeConditionMet(learningDelivery.FundModelNullable);
        }

        public bool HasLearningDeliveryDd07ExcludeConditionMet(string dd07)
        {
            return dd07 == ValidationConstants.Y;
        }

        public bool HasLearningDeliveryFundModelExcludeConditionMet(long? fundModelNullable)
        {
            return fundModelNullable.HasValue
                && fundModelNullable.Value == 70;
        }

        public bool HasAllLearningAimsClosedExcludeConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                && learningDeliveries.All(x => x.LearnActEndDateNullable.HasValue);
        }
    }
}