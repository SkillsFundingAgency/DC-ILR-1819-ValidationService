using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_54Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_18Rule _derivedData18;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IAcademicYearQueryService _academicYearQueryService;

        private readonly DateTime _firstAugust2016 = new DateTime(2016, 08, 01);

        public DateOfBirth_54Rule(
            IDerivedData_18Rule derivedData18,
            IDerivedData_07Rule derivedData07,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_54)
        {
            _derivedData18 = derivedData18;
            _derivedData07 = derivedData07;
            _academicYearQueryService = academicYearQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null || !objectToValidate.DateOfBirthNullable.HasValue)
            {
                return;
            }

            var sixteenthBirthDate = objectToValidate.DateOfBirthNullable.Value.AddYears(16);
            var lastFridayInJuneForAcademicYear = _academicYearQueryService.LastFridayInJuneForDateInAcademicYear(sixteenthBirthDate);

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                DateTime? dd18Date = _derivedData18.GetApprenticeshipStandardProgrammeStartDateFor(learningDelivery, objectToValidate.LearningDeliveries);

                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    dd18Date,
                    lastFridayInJuneForAcademicYear))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? progType, DateTime? dd18Date, DateTime lastFridayInJuneForAcademicYear)
        {
            return (progType.HasValue && progType == TypeOfLearningProgramme.ApprenticeshipStandard)
                   && (dd18Date.HasValue && dd18Date >= _firstAugust2016)
                   && dd18Date <= lastFridayInJuneForAcademicYear
                   && _derivedData07.IsApprenticeship(progType);
        }
    }
}
