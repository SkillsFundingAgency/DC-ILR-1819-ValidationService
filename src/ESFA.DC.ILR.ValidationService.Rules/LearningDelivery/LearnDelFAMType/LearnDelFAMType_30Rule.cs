using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_30Rule : AbstractRule, IRule<ILearner>
    {
        public LearnDelFAMType_30Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_30)
        {
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs == null || learningDelivery.LearningDeliveryHEEntity != null)
                {
                    continue;
                }

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (deliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.HEM))
                    {
                        RaiseValidationMessage(learner.LearnRefNumber, learningDelivery, deliveryFam);
                    }
                }
            }
        }

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery learningDelivery, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType)
            };

            HandleValidationError(learnRefNum, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
