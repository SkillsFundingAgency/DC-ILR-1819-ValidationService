using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_02Rule : AbstractRule, IRule<ILearner>
    {
        public WorkPlaceStartDate_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceStartDate_02)
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
                if (learningDelivery.LearningDeliveryWorkPlacements != null)
                {
                    foreach (var workPlacement in learningDelivery.LearningDeliveryWorkPlacements)
                    {
                        if (ConditionMet(learningDelivery.LearnStartDate, workPlacement.WorkPlaceStartDate))
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(learningDelivery.LearnStartDate, workPlacement.WorkPlaceStartDate));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, DateTime workPlaceStarDate)
        {
            return workPlaceStarDate < learnStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime workPlaceStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, workPlaceStartDate)
            };
        }
    }
}
