using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpId_01";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The employer data reference service
        /// </summary>
        private readonly IProvideEDRSDataOperations _edrsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="edrsData">The employer data reference service.</param>
        public EmpId_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideEDRSDataOperations edrsData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(edrsData)
                .AsGuard<ArgumentNullException>(nameof(edrsData));

            _messageHandler = validationErrorHandler;
            _edrsData = edrsData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        public bool IsNotValid(ILearnerEmploymentStatus employment) =>
            !_edrsData.IsValid(employment.EmpIdNullable);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learner reference number.</param>
        /// <param name="thisEmployment">this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.EmpId, thisEmployment.EmpIdNullable));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}