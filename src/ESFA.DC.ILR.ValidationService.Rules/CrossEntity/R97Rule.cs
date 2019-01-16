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
            foreach (var learnerEmpoyementStatus in objectToValidate.LearnerEmploymentStatuses.OrderBy(e => e.DateEmpStatApp))
            {
                if (record > 1)
                {
                }

                record++;
            }

            if (true)
            {
                //HandleValidationError(
                //    learnRefNumber: objectToValidate.LearnRefNumber,
                //    errorMessageParameters: BuildErrorMessageParameter(
                //        learnerEmpoyementStatus.EmpStat,
                //        learnerEmpoyementStatus.DateEmpStatApp,
                //        learnerEmpoyementStatus.EmpIdNullable,
                //        learnerEmpoyementStatus.EmploymentStatusMonitorings.FirstOrDefault().ESMType,
                //        learnerEmpoyementStatus.EmploymentStatusMonitorings.FirstOrDefault().ESMCode));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameter(
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
