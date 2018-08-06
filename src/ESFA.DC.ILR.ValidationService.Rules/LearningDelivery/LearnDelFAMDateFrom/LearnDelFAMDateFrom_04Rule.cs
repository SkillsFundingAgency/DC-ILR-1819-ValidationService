using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom
{
    public class LearnDelFAMDateFrom_04Rule : AbstractRule, IRule<ILearner>
    {
        public LearnDelFAMDateFrom_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateFrom_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs != null)
                {
                    foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                    {
                        if (ConditionMet(learningDeliveryFam.LearnDelFAMDateFromNullable, learningDeliveryFam.LearnDelFAMType))
                        {
                            HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDeliveryFam.LearnDelFAMType, learningDeliveryFam.LearnDelFAMDateFromNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? learnDelFamDateFrom, string learnDelFamType)
        {
            return !learnDelFamDateFrom.HasValue && learnDelFamType == LearningDeliveryFAMTypeConstants.ACT;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFamType, DateTime? learnDelFamDateFrom)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom)
            };
        }
    }
}
