using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEndDate
{
    public class WorkPlaceEndDate_01Rule : AbstractRule, IRule<ILearner>
    {
        public WorkPlaceEndDate_01Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceEndDate_01)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryWorkPlacements == null)
                {
                    continue;
                }

                foreach (var workPlacement in learningDelivery.LearningDeliveryWorkPlacements)
                {
                    if ((workPlacement.WorkPlaceEndDateNullable ?? DateTime.MaxValue) < workPlacement.WorkPlaceStartDate)
                    {
                        HandleValidationError(
                            learner.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(workPlacement));
                    }
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDeliveryWorkPlacement workPlacement)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, workPlacement.WorkPlaceStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEndDate, workPlacement.WorkPlaceEndDateNullable)
            };
        }
    }
}
