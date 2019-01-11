using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_50Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _julyThirtyFirst2016 = new DateTime(2016, 7, 31);

        public DateOfBirth_50Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_50)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (objectToValidate.DateOfBirthNullable.HasValue)
            {
                var learnersSixteenthBirthdate = objectToValidate.DateOfBirthNullable.Value.AddYears(16);
                var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = FirstAugustForDateInAcademicYear(learnersSixteenthBirthdate);

                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery,
                        firstAugustForAcademicYearOfLearnersSixteenthBirthDate))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.LearnStartDate));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, DateTime firstAugustForAcademicYearOfLearnersSixteenthBirthDate)
        {
            return (learningDelivery.ProgTypeNullable.HasValue && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship)
                   && learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.LearnStartDate > _julyThirtyFirst2016
                   && learningDelivery.LearnStartDate < firstAugustForAcademicYearOfLearnersSixteenthBirthDate;
        }

        public DateTime FirstAugustForDateInAcademicYear(DateTime dateTime)
        {
            return dateTime.Month > 8 ? new DateTime(dateTime.Year + 1, 8, 1) : new DateTime(dateTime.Year, 8, 1);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
