using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate
{
    public class LearnActEndDate_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;

        public LearnActEndDate_04Rule(IFileDataService fileDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnActEndDate_04)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePreparationDate = _fileDataService.FilePreparationDate();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(filePreparationDate, learningDelivery.LearnActEndDateNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime filePreparationDate, DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue
                   && learnActEndDate > filePreparationDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
            };
        }
    }
}
