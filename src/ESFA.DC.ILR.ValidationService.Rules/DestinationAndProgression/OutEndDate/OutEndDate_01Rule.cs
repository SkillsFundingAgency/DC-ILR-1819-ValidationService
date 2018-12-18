using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutEndDate
{
    public class OutEndDate_01Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        public OutEndDate_01Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutEndDate_01)
        {
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate?.DPOutcomes == null)
            {
                return;
            }

            foreach (var dpOutcome in objectToValidate.DPOutcomes)
            {
                if (ConditionMet(dpOutcome.OutStartDate, dpOutcome.OutEndDateNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(dpOutcome.OutStartDate, dpOutcome.OutEndDateNullable));
                }
            }
        }

        private bool ConditionMet(DateTime outStartDate, DateTime? outEndDate)
        {
            return outEndDate != null && outStartDate > outEndDate;
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime outStartDate, DateTime? outEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.OutEndDate, outEndDate)
            };
        }
    }
}
