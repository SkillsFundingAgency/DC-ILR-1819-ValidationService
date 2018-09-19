using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_05Rule : AbstractRule, IRule<ILearner>
    {
        public AchDate_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, learningDelivery.LearnActEndDateNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable, learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, DateTime? learnActEndDate)
        {
            return achDate.HasValue && achDate < learnActEndDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
