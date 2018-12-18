using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate
{
    public class OutCollDate_02Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IAcademicYearDataService _academicYearDataService;

        public OutCollDate_02Rule(
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutCollDate_02)
        {
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate?.DPOutcomes == null)
            {
                return;
            }

            var academicStartMinus10Years = _academicYearDataService.Start().AddYears(-10);

            foreach (var dpOutcome in objectToValidate.DPOutcomes)
            {
                if (ConditionMet(dpOutcome.OutCollDate, academicStartMinus10Years))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(dpOutcome.OutCollDate));
                }
            }
        }

        private bool ConditionMet(DateTime outCollDate, DateTime academicStartMinus10Years)
        {
            return outCollDate < academicStartMinus10Years;
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime outCollDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutCollDate, outCollDate),
            };
        }
    }
}
