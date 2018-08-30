using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_48Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2016 = new DateTime(2016, 08, 01);
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IDateTimeQueryService _datetimeQueryService;
        private readonly IDD07 _dd07;
        private readonly IDD04 _dd04;

        public DateOfBirth_48Rule(
            IDD07 dd07,
            IDD04 dd04,
            IAcademicYearQueryService academicYearQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_48)
        {
            _dd07 = dd07;
            _dd04 = dd04;
            _academicYearQueryService = academicYearQueryService;
            _datetimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null
                || LearnerConditionMet(objectToValidate.DateOfBirthNullable))
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                DateTime? dd04Date = _dd04.Derive(objectToValidate.LearningDeliveries, learningDelivery);
                DateTime sixteenthBirthDate = _datetimeQueryService.DateAddYears((DateTime)objectToValidate.DateOfBirthNullable, 16);
                DateTime lastFridayInJuneForAcademicYear = _academicYearQueryService.LastFridayInJuneForDateInAcademicYear(sixteenthBirthDate);

                if (ConditionMet(learningDelivery.ProgTypeNullable, dd04Date, lastFridayInJuneForAcademicYear))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int? progType, DateTime? dd04Date, DateTime lastFridayInJune)
        {
            return DD07ConditionMet(progType)
                && DD04ConditionMet(dd04Date, lastFridayInJune);
        }

        public bool LearnerConditionMet(DateTime? dateOfBirth)
        {
            return !dateOfBirth.HasValue;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType.HasValue
                && progType != 25
                && _dd07.IsApprenticeship(progType);
        }

        public bool DD04ConditionMet(DateTime? dd04, DateTime lastFridayJuneAcademicYearLearnerSixteen)
        {
            return dd04.HasValue
                && dd04 >= _augustFirst2016
                && dd04 <= lastFridayJuneAcademicYearLearnerSixteen;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth?.ToString("d", new CultureInfo("en-GB"))),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", new CultureInfo("en-GB")))
            };
        }
    }
}
