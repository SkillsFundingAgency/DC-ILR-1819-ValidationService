﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famCode = "118";

        public LearnDelFAMType_15Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_15)
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
                if (learningDelivery.LearningDeliveryFAMs == null || learningDelivery.LearnActEndDateNullable == null)
                {
                    continue;
                }

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (deliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.LDM)
                        && deliveryFam.LearnDelFAMCode.CaseInsensitiveEquals(_famCode))
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
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, thisMonitor.LearnDelFAMCode)
            };

            HandleValidationError(learnRefNum, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
