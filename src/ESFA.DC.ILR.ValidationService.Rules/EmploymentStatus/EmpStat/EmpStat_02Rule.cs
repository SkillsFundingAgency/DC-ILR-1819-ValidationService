using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _julyThirtyFirst2014 = new DateTime(2014, 07, 31);

        private readonly IDerivedData_07Rule _dd07;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_02Rule(
            IDerivedData_07Rule dd07,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_02)
        {
            _dd07 = dd07;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
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
                    learningDelivery.AimType,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable,
                    objectToValidate.LearnerEmploymentStatuses))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(
            int aimType,
            DateTime learnStartDate,
            int? progType,
            IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return AimTypeConditionMet(aimType)
                && LearnStartDateConditionMet(learnStartDate)
                && ApprenticeshipConditionMet(progType)
                && EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate <= _julyThirtyFirst2014;
        }

        public bool ApprenticeshipConditionMet(int? progType)
        {
            return progType.HasValue ? progType == 24 || _dd07.IsApprenticeship(progType) : false;
        }

        public bool EmploymentStatusConditionMet(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime learnStartDate)
        {
            return _learnerEmploymentStatusQueryService.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
