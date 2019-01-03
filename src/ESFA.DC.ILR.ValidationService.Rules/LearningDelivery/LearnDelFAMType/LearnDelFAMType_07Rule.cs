using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _fundingModels = { 25, 82 };
        private readonly string[] _famCodesForSOFType = { "105", "107" };

        public LearnDelFAMType_07Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_07)
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
                    if (deliveryFam.LearnDelFAMType != LearningDeliveryFAMTypeConstants.SOF)
                    {
                        continue;
                    }

                    if (learningDelivery.LearningDeliveryFAMs
                        .All(fam => !_famCodesForSOFType.Contains(fam.LearnDelFAMCode)))
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
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, thisMonitor.LearnDelFAMCode)
            };

            HandleValidationError(learnRefNumber, aimSeqNumber, parameters);
        }
    }
}
