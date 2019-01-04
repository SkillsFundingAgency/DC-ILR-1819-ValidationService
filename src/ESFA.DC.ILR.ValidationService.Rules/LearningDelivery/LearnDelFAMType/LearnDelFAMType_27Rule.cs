using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_27Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _fundingModels = { 25, 82, 35, 36, 81, 70 };

        public LearnDelFAMType_27Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_27)
        {
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!_fundingModels.Contains(learningDelivery.FundModel)
                    || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (deliveryFam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ASL)
                    {
                        RaiseValidationMessage(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, learningDelivery.FundModel, deliveryFam);
                    }
                }
            }
        }

        private void RaiseValidationMessage(string learnRefNumber, long? aimSeqNumber, int fundingModel, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundingModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType)
            };

            HandleValidationError(learnRefNumber, aimSeqNumber, parameters);
        }
    }
}
