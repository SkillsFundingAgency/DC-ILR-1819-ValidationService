﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_04Rule : AbstractRule, IRule<ILearner>
    {
        public WorkPlaceStartDate_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceStartDate_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int fundModel)
        {
            return string.Equals(learnAimRef, TypeOfAim.References.IndustryPlacement, StringComparison.OrdinalIgnoreCase) && fundModel != TypeOfFunding.Age16To19ExcludingApprenticeships;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
