using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_05Rule _derivedData05;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="edrsData">The employer data reference service.</param>
        /// <param name="derivedData05">The derived data (rule) 05.</param>
        public EmpId_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_05Rule derivedData05)
            : base(validationErrorHandler, RuleNameConstants.EmpId_02)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData05)
                .AsGuard<ArgumentNullException>(nameof(derivedData05));

            _derivedData05 = derivedData05;
        }

        /// <summary>
        /// Determines whether [has valid checksum] [the specified employer identifier].
        /// </summary>
        /// <param name="employerID">The employer identifier.</param>
        /// <returns>
        ///   <c>true</c> if [has valid checksum] [the specified employer identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidChecksum(int employerID)
        {
            var checkSum = _derivedData05.GetEmployerIDChecksum(employerID);

            if (It.IsInRange(checkSum, _derivedData05.InvalidLengthChecksum))
            {
                return false;
            }

            var candidate = employerID.AsSafeReadOnlyDigitList().ElementAt(8).ToString();
            return candidate.ComparesWith(checkSum.ToString());
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified employment].
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employment) =>
            employment.EmpIdNullable.HasValue
                && employment.EmpIdNullable != ValidationConstants.TemporaryEmployerId
                && !HasValidChecksum(employment.EmpIdNullable.Value);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (objectToValidate.LearnerEmploymentStatuses != null)
            {
                foreach (var learnerEmploymentStatus in objectToValidate.LearnerEmploymentStatuses)
                {
                    if (IsNotValid(learnerEmploymentStatus))
                    {
                        RaiseValidationMessage(learnRefNumber, learnerEmploymentStatus);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learner reference number.</param>
        /// <param name="thisEmployment">this employment.</param>
        private void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();

            parameters.Add(BuildErrorMessageParameter(PropertyNameConstants.EmpId, thisEmployment.EmpIdNullable));

            HandleValidationError(learnRefNumber, null, parameters);
        }
    }
}