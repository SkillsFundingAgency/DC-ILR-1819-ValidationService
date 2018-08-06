using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataCache _fileDataCache;

        public AchDate_07Rule(IFileDataCache fileDataCache, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_07)
        {
            _fileDataCache = fileDataCache;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, _fileDataCache.FilePreparationDate))
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
