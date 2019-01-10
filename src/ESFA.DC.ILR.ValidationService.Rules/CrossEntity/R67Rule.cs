using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R67Rule : AbstractRule, IRule<ILearner>
    {
        public R67Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R67)
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
                    var duplicates = learningDelivery.LearningDeliveryWorkPlacements.GroupBy(
                            x => new
                            {
                                x.WorkPlaceEmpIdNullable,
                                x.WorkPlaceStartDate
                            })
                        .Where(dup => dup.Count() > 1);

                    foreach (var workPlacement in duplicates)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                workPlacement.Key.WorkPlaceStartDate,
                                workPlacement.Key.WorkPlaceEmpIdNullable));
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? workPlaceStartDate, int? empId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, workPlaceStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, empId)
            };
        }
    }
}