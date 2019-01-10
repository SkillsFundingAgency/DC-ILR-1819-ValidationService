using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2014 = new DateTime(2014, 08, 01);
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.NotFundedByESFA };

        private readonly IDD07 _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_08Rule(
            IDD07 dd07,
            IDateTimeQueryService dateTimeQueryService,
            IAcademicYearQueryService academicYearQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_08)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _academicYearQueryService = academicYearQueryService;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    objectToValidate.DateOfBirthNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable,
                    objectToValidate.LearnerEmploymentStatuses,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(
            int fundModel,
            DateTime? dateOfBirth,
            DateTime learnStartDate,
            int? progType,
            IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && LearningDeliveryConditionMet(dateOfBirth, learnStartDate)
                && EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate)
                && DD07ConditionMet(progType)
                && LearningDeliveryFAMsConditionMet(fundModel, learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && learnStartDate >= _augustFirst2014
                && _dateTimeQueryService.YearsBetween((DateTime)dateOfBirth, _academicYearQueryService.AugustThirtyFirstOfLearnStartDate(learnStartDate)) >= 19;
        }

        public bool EmploymentStatusConditionMet(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime learnStartDate)
        {
            return _learnerEmploymentStatusQueryService.EmpStatsNotExistBeforeLearnStartDate(learnerEmploymentStatuses, learnStartDate);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !progType.HasValue
                || (progType.HasValue
                && progType != 24
                && !_dd07.IsApprenticeship(progType));
        }

        public bool LearningDeliveryFAMsConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")
                || (fundModel != 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
