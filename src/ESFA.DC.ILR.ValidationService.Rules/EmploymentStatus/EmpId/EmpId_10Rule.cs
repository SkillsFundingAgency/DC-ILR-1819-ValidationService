using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_10Rule : AbstractRule, IRule<ILearner>
    {
        private const int _empStat = TypeOfEmploymentStatus.InPaidEmployment;
        private readonly IDerivedData_07Rule _dd07;

        public EmpId_10Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpId_10)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            var employmentStatuses = EmploymentStatusesInPaidEmployment(objectToValidate.LearnerEmploymentStatuses).ToList();

            if (objectToValidate.LearningDeliveries == null || !employmentStatuses.Any())
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable, learningDelivery.AimType, learningDelivery.LearnStartDate, employmentStatuses))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(_empStat));
                }
            }
        }

        public bool ConditionMet(int? progType, int aimType, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return DD07ConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ProgrammeAim;
        }

        public bool EmpIdNotExistsOnLearnStartDate(DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            var employmentStatuses = learnerEmploymentStatuses?
                .Where(l => l.DateEmpStatApp <= learnStartDate).ToList() ?? new List<ILearnerEmploymentStatus>();

            return employmentStatuses.Any()
                    ? !employmentStatuses.Any(l => l.EmpIdNullable.HasValue)
                    : false;
        }

        public IEnumerable<ILearnerEmploymentStatus> EmploymentStatusesInPaidEmployment(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learnerEmploymentStatuses?.Where(l => l.EmpStat == _empStat) ?? new List<ILearnerEmploymentStatus>();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int empStat)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, string.Empty)
            };
        }
    }
}
