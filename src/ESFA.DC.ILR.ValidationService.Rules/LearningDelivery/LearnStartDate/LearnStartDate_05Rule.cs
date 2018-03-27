using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_05Rule : AbstractRule, IRule<ILearner>
    {
        public LearnStartDate_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.DateOfBirthNullable, learningDelivery.LearnStartDateNullable))
                {
                    HandleValidationError(RuleNameConstants.LearnStartDate_05, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, DateTime? learnStartDate)
        {
            return dateOfBirth.HasValue
                && learnStartDate.HasValue
                && dateOfBirth.Value >= learnStartDate;
        }
    }
}
