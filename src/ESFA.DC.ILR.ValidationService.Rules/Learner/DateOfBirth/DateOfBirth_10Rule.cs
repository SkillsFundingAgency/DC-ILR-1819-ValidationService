using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD04 _dd04;
        private readonly IDD07 _dd07;
        private readonly IAcademicYearCalendarService _academicYearCalendarService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly DateTime _apprenticeshipProgrammeStartStart = new DateTime(2013, 8, 1);
        private readonly DateTime _apprenticeshipProgrammeStartEnd = new DateTime(2015, 8, 1);

        public DateOfBirth_10Rule(IDD04 dd04, IDD07 dd07, IAcademicYearCalendarService academicYearCalendarService, IDateTimeQueryService dateTimeQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _dd04 = dd04;
            _dd07 = dd07;
            _academicYearCalendarService = academicYearCalendarService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (LearnerNullConditionMet(objectToValidate.DateOfBirthNullable))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    var dd04 = _dd04.Derive(objectToValidate.LearningDeliveries, learningDelivery);

                    if (LearningDeliveryNullConditionMet(dd04, learningDelivery.LearnStartDateNullable)
                        && DD04ConditionMet(dd04.Value)
                        && DD07ConditionMet(_dd07.Derive(learningDelivery.ProgTypeNullable))
                        && DateOfBirthLearnStartDateConditionMet(objectToValidate.DateOfBirthNullable.Value, learningDelivery.LearnStartDateNullable.Value))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_10, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool LearnerNullConditionMet(DateTime? dateOfBirth)
        {
            return dateOfBirth.HasValue;
        }

        public bool LearningDeliveryNullConditionMet(DateTime? dd04, DateTime? learnStartDate)
        {
            return dd04.HasValue && learnStartDate.HasValue;
        }

        public bool DD04ConditionMet(DateTime dd04)
        {
            return dd04 >= _apprenticeshipProgrammeStartStart
                && dd04 < _apprenticeshipProgrammeStartEnd;
        }

        public bool DD07ConditionMet(string dd07)
        {
            return dd07 == ValidationConstants.Y;
        }

        public bool DateOfBirthLearnStartDateConditionMet(DateTime learnStartDate, DateTime dateOfBirth)
        {
            var learnStartDateAcademicYearLastFridayJune = _academicYearCalendarService.LastFridayInJuneForDateInAcademicYear(learnStartDate);
            var learnStartDateFirstSeptember = new DateTime(learnStartDateAcademicYearLastFridayJune.Year, 9, 1);
            var sixteenthBirthday = dateOfBirth.AddYears(16);
            var learnStartDateAge = _dateTimeQueryService.YearsBetween(dateOfBirth, learnStartDate);

            return learnStartDateAge < 16
                &&
                !(learnStartDateAge == 15
                && sixteenthBirthday > learnStartDateAcademicYearLastFridayJune && sixteenthBirthday < learnStartDateFirstSeptember
                && learnStartDate > learnStartDateAcademicYearLastFridayJune && learnStartDate < learnStartDateFirstSeptember);
        }
    }
}