using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;

        public AchDate_07Rule(IFileDataService fileDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_07)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePrepDate = _fileDataService.FilePreparationDate();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, filePrepDate))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, DateTime filePreparationDate)
        {
            return achDate.HasValue && achDate > filePreparationDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
