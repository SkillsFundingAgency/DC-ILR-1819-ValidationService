using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_01Rule : AbstractRule, IRule<ILearner>
    {
        /// <summary>
        /// The employer data reference service
        /// </summary>
        private readonly IEmployersDataService _edrsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="edrsData">The employer data reference service.</param>
        public EmpId_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IEmployersDataService edrsData)
            : base(validationErrorHandler, RuleNameConstants.EmpId_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(edrsData)
                .AsGuard<ArgumentNullException>(nameof(edrsData));

            _edrsData = edrsData;
        }

        public bool IsNotValid(int? empId) =>
            empId.HasValue
            && empId != ValidationConstants.TemporaryEmployerId
            && !_edrsData.IsValid(empId);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner.LearnerEmploymentStatuses != null)
            {
                foreach (var learnerEmploymentStatus in learner.LearnerEmploymentStatuses)
                {
                    if (IsNotValid(learnerEmploymentStatus.EmpIdNullable))
                    {
                        RaiseValidationMessage(learner.LearnRefNumber, learnerEmploymentStatus);
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