using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate
{
    public class OutCollDate_01Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IFileDataService _fileDataService;
        private DateTime? _outCollDateForError;

        public OutCollDate_01Rule(IFileDataService fileDataService, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.OutCollDate_01)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (ConditionMet(objectToValidate.DPOutcomes))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(_outCollDateForError));
            }
        }

        public bool ConditionMet(IEnumerable<IDPOutcome> dpOutcomes)
        {
            _outCollDateForError = dpOutcomes.Where(dp => dp.OutCollDate > _fileDataService.FilePreparationDate()).Select(o => o.OutCollDate).FirstOrDefault();

            return _outCollDateForError != DateTime.MinValue ? true : false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? outCollDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutCollDate, outCollDate),
            };
        }
    }
}
