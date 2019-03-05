using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_04Rule : AbstractRule, IRule<ILearner>
    {
        public Outcome_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AchDateNullable,
                    learningDelivery.OutcomeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.AchDateNullable, learningDelivery.OutcomeNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, int? outcome)
        {
            return AchDateConditionMet(achDate)
                   && OutcomeConditionMet(outcome);
        }

        public bool AchDateConditionMet(DateTime? achDate)
        {
            return achDate.HasValue;
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome != 1;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? achDate, int? outcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome)
            };
        }
    }
}
