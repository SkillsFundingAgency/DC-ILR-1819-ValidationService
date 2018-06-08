using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom
{
    public class LearnDelFAMDateFrom_01Rule : AbstractRule, IRule<ILearner>
    {
        public LearnDelFAMDateFrom_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateFrom_01)
        {
        }

        public LearnDelFAMDateFrom_01Rule()
            : base(null, null)
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
                        if (ConditionMet(learningDeliveryFam.LearnDelFAMType, learningDeliveryFam.LearnDelFAMDateFromNullable, learningDeliveryFam.LearnDelFAMDateToNullable))
                        {
                            HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDeliveryFam.LearnDelFAMType, learningDeliveryFam.LearnDelFAMDateFromNullable, learningDeliveryFam.LearnDelFAMDateToNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(string learnDelFamType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return LearnDelFAMTypeConditionMet(learnDelFamType) && LearnDelFAMDatesConditionMet(learnDelFamDateFrom, learnDelFamDateTo);
        }

        public virtual bool LearnDelFAMTypeConditionMet(string learnDelFamType)
        {
            return learnDelFamType == LearningDeliveryFAMTypeConstants.LSF || learnDelFamType == LearningDeliveryFAMTypeConstants.ALB;
        }

        public virtual bool LearnDelFAMDatesConditionMet(DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return !learnDelFamDateFrom.HasValue || !learnDelFamDateTo.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFamType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo),
            };
        }
    }
}
