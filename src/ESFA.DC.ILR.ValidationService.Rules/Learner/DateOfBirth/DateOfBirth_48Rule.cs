using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_48Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD04 _dd04;
        private readonly IDD07 _dd07;
        private readonly IValidationDataService _validationDataService;
        private readonly IAcademicYearCalendarService _academicYearCalendarService;

        public DateOfBirth_48Rule(IDD04 dd04, IDD07 dd07, IValidationDataService validationDataService, IAcademicYearCalendarService academicYearCalendarService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _dd04 = dd04;
            _dd07 = dd07;
            _validationDataService = validationDataService;
            _academicYearCalendarService = academicYearCalendarService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (!LearnerConditionMet(objectToValidate.DateOfBirthNullable))
            {
                return;
            }

            if (objectToValidate.LearningDeliveries != null)
            {
                var sixteenthBirthday = BirthdayAt(objectToValidate.DateOfBirthNullable, 16);
                var lastFridayJuneAcademicYearLearnerSixteen =
                    _academicYearCalendarService.LastFridayInJuneForDateInAcademicYear(sixteenthBirthday.Value);

                foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld =>
                    !Exclude(ld.ProgTypeNullable)))
                {
                    if (DD07ConditionMet(_dd07.Derive(learningDelivery.ProgTypeNullable))
                        && DD04ConditionMet(_dd04.Derive(objectToValidate.LearningDeliveries, learningDelivery), _validationDataService.ApprencticeProgAllowedStartDate, lastFridayJuneAcademicYearLearnerSixteen))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_48, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool Exclude(long? progType)
        {
            return progType == 25;
        }

        public bool LearnerConditionMet(DateTime? dateOfBirth)
        {
            return dateOfBirth.HasValue;
        }

        public bool DD07ConditionMet(string dd07)
        {
            return dd07 == ValidationConstants.Y;
        }

        public bool DD04ConditionMet(DateTime? dd04, DateTime apprenticeshipProgrammeAllowedStartDate, DateTime lastFridayJuneAcademicYearLearnerSixteen)
        {
            return dd04.HasValue
                && dd04 >= apprenticeshipProgrammeAllowedStartDate
                && dd04 <= lastFridayJuneAcademicYearLearnerSixteen;
        }

        public DateTime? BirthdayAt(DateTime? dateOfBirth, int age)
        {
            return dateOfBirth?.AddYears(age);
        }
    }
}