using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_10Rule : AbstractRule, IRule<ILearner>
    {
        public AchDate_10Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_10)
        {
        }

        public AchDate_10Rule()
            : base(null, RuleNameConstants.AchDate_10)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, learningDelivery.LearnActEndDateNullable, learningDelivery.AimType, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.AimType,
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.LearnActEndDateNullable,
                            learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, DateTime? learnActEndDate, int aimType, int? progType)
        {
            return TraineeshipConditionMet(aimType, progType)
                   && AchDateConditionMet(achDate, learnActEndDate);
        }

        public virtual bool AchDateConditionMet(DateTime? achDate, DateTime? learnActEndDate)
        {
            return achDate.HasValue
                   && learnActEndDate.HasValue
                   && achDate.Value > learnActEndDate.Value.AddMonths(6);
        }

        public virtual bool TraineeshipConditionMet(int aimType, int? progType)
        {
            return aimType == 1 && progType == 24;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int? progType, DateTime? learnActEndDate, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
