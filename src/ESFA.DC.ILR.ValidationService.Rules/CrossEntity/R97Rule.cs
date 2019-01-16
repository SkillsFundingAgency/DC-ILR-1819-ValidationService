using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R97Rule : AbstractRule, IRule<ILearner>
    {
        public R97Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R97)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerEmploymentStatuses == null)
            {
                return;
            }

            int record = 1;
            ILearnerEmploymentStatus previousLearnerEmploymentStatus = null;
            foreach (var learnerEmploymentStatus in
                objectToValidate.LearnerEmploymentStatuses.OrderBy(e => e.DateEmpStatApp))
            {
                if (learnerEmploymentStatus == null)
                {
                    continue;
                }

                if (record > 1)
                {
                    if (LearnerEmploymentStatusConditionMet(
                        previousLearnerEmploymentStatus,
                        learnerEmploymentStatus))
                    {
                        var employmentStatusMonitoring = LearnerEmploymentStatusMonitoringConditionMet(
                            previousLearnerEmploymentStatus.EmploymentStatusMonitorings,
                            learnerEmploymentStatus.EmploymentStatusMonitorings);

                        if (employmentStatusMonitoring != null)
                        {
                            HandleValidationError(
                                learnRefNumber: objectToValidate.LearnRefNumber,
                                errorMessageParameters: BuildErrorMessageParameters(
                                    learnerEmploymentStatus.EmpStat,
                                    learnerEmploymentStatus.DateEmpStatApp,
                                    learnerEmploymentStatus.EmpIdNullable,
                                    employmentStatusMonitoring.ESMType,
                                    employmentStatusMonitoring.ESMCode));
                        }
                    }
                }

                previousLearnerEmploymentStatus = learnerEmploymentStatus;
                record++;
            }
        }

        public IEmploymentStatusMonitoring LearnerEmploymentStatusMonitoringConditionMet(
            IReadOnlyCollection<IEmploymentStatusMonitoring> previousEmploymentStatusMonitorings,
            IReadOnlyCollection<IEmploymentStatusMonitoring> employmentStatusMonitorings)
        {
            if (previousEmploymentStatusMonitorings != null
                && employmentStatusMonitorings != null)
            {
                return previousEmploymentStatusMonitorings.Join(
                    employmentStatusMonitorings,
                    previous => new { previous.ESMType, previous.ESMCode },
                    current => new { current.ESMType, current.ESMCode },
                    (previous, current) => previous).FirstOrDefault();
            }

            return null;
        }

        public bool LearnerEmploymentStatusConditionMet(
            ILearnerEmploymentStatus previousLearnerEmploymentStatus,
            ILearnerEmploymentStatus learnerEmploymentStatus) => learnerEmploymentStatus.AgreeId == previousLearnerEmploymentStatus.AgreeId
                && learnerEmploymentStatus.EmpStat == previousLearnerEmploymentStatus.EmpStat
                && learnerEmploymentStatus.EmpIdNullable == previousLearnerEmploymentStatus.EmpIdNullable;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int empStat,
            DateTime dateEmpStatApp,
            int? empId,
            string eSMType,
            int eSMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, dateEmpStatApp),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, empId),
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, eSMType),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, eSMCode)
            };
        }
    }
}
