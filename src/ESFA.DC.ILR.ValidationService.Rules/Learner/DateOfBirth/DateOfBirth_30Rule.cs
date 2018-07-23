using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_30Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { 25, 35, 81, 82 };

        private readonly IDD07 _dd07;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_30Rule(IDD07 dd07, IAcademicYearDataService academicYearDataService, IDateTimeQueryService dateTimeQueryService, ILearnerFAMQueryService learnerFAMQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_30)
        {
            _dd07 = dd07;
            _academicYearDataService = academicYearDataService;
            _dateTimeQueryService = dateTimeQueryService;
            _learnerFAMQueryService = learnerFAMQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    objectToValidate.DateOfBirthNullable,
                    objectToValidate.LearnerFAMs,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.FundModel));
                    return;
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime? dateOfBirth, IEnumerable<ILearnerFAM> learnerFAMs, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && DD07ConditionMet(progType)
                && DateOfBirthConditionMet(dateOfBirth)
                && LearnerFAMConditionMet(learnerFAMs)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType.HasValue
                && !_dd07.IsApprenticeship(progType);
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth)
        {
            if (dateOfBirth.HasValue)
            {
                var augustThirtyFirstAge = _dateTimeQueryService.YearsBetween(dateOfBirth.Value, _academicYearDataService.AugustThirtyFirst());

                return augustThirtyFirstAge >= 19 && augustThirtyFirstAge <= 24;
            }

            return false;
        }

        public bool LearnerFAMConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnerFAMQueryService.HasLearnerFAMType(learnerFAMs, "EHC");
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return (_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "107"))
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}