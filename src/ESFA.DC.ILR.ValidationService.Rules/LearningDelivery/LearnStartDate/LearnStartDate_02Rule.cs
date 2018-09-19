using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;

        public LearnStartDate_02Rule(IValidationDataService validationDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _validationDataService = validationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnStartDateNullable, _validationDataService.AcademicYearStart))
                {
                    HandleValidationError(RuleNameConstants.LearnStartDate_02, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(DateTime? learnStartDate, DateTime academicYearStart)
        {
            return learnStartDate.HasValue
                && learnStartDate < academicYearStart.AddYears(-10);
        }
    }
}
