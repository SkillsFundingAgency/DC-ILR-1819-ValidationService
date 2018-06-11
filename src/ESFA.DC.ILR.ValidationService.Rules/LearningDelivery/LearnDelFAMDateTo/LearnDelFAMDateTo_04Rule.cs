using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo
{
    public class LearnDelFAMDateTo_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<string> _learnDelFamTypes = new HashSet<string>()
        {
            LearningDeliveryFAMTypeConstants.LSF,
            LearningDeliveryFAMTypeConstants.ACT,
            LearningDeliveryFAMTypeConstants.ALB
        };

        public LearnDelFAMDateTo_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateTo_04)
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
                        if (ConditionMet(learningDeliveryFam.LearnDelFAMDateToNullable, learningDeliveryFam.LearnDelFAMType))
                        {
                            HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDeliveryFam.LearnDelFAMDateToNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? learnDelFamDateTo, string learnDelFamType)
        {
            return learnDelFamDateTo.HasValue && !_learnDelFamTypes.Contains(learnDelFamType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
