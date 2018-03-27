using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public DateOfBirth_03Rule(IValidationDataService validationDataService, IDateTimeQueryService dateTimeQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _validationDataService = validationDataService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.DateOfBirthNullable, _validationDataService.AcademicYearStart))
            {
                HandleValidationError(RuleNameConstants.DateOfBirth_03, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, DateTime academicYearStart)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearStart) >= 100;
        }
    }
}