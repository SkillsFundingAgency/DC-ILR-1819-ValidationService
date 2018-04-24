using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate
{
    public class LearnActEndDate_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataCache _fileDataCache;

        public LearnActEndDate_04Rule(IFileDataCache fileDataCache, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnActEndDate_04)
        {
            _fileDataCache = fileDataCache;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePreparationDate = _fileDataCache.FilePreparationDate;

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
