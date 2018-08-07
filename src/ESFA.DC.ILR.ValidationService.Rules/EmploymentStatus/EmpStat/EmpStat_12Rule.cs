using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_12Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _famCOdes = new HashSet<string> { "353", "354", "355" };

        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;
        private readonly IDD07 _dd07;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private int _empStat;

        public EmpStat_12Rule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            IDD07 dd07,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_12)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
            _dd07 = dd07;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnStartDate,
                    objectToValidate.LearnerEmploymentStatuses,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(_empStat, learningDelivery.LearnStartDate));
                    return;
                }
            }
        }

        public bool ConditionMet(
            int? progType,
            int aimType,
            DateTime learnStartDate,
            IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return AimTypeConditionMet(aimType)
                && DD07ConditionMet(progType)
                && EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool EmpStatConditionMet(DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (learnerEmploymentStatuses != null)
            {
                var empStat = _learnerEmploymentStatusQueryService.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate);

                if (empStat.HasValue && empStat != 10)
                {
                    _empStat = empStat.Value;
                    return true;
                }
            }

            return false;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _famCOdes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int empStat, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", new CultureInfo("en-GB")))
            };
        }
    }
}
