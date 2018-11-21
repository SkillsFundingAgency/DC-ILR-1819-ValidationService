using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpId_02";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The employer data reference service
        /// </summary>
        private readonly IProvideEDRSDataOperations _edrsData;

        /// <summary>
        /// The derived data (rule) 05
        /// </summary>
        private readonly IDerivedData_05Rule _derivedData05;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="edrsData">The employer data reference service.</param>
        /// <param name="derivedData05">The derived data (rule) 05.</param>
        public EmpId_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideEDRSDataOperations edrsData,
            IDerivedData_05Rule derivedData05)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(edrsData)
                .AsGuard<ArgumentNullException>(nameof(edrsData));
            It.IsNull(derivedData05)
                .AsGuard<ArgumentNullException>(nameof(derivedData05));

            _messageHandler = validationErrorHandler;
            _edrsData = edrsData;
            _derivedData05 = derivedData05;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [has valid checksum] [the specified employer identifier].
        /// </summary>
        /// <param name="employerID">The employer identifier.</param>
        /// <returns>
        ///   <c>true</c> if [has valid checksum] [the specified employer identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidChecksum(int employerID)
        {
            var checkSum = _derivedData05.GetEmployerIDChecksum(employerID).ToString();
            var candidate = employerID.AsSafeReadOnlyDigitList().Skip(8).First().ToString();
            return candidate.ComparesWith(checkSum);
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified employment].
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employment) =>
            It.Has(employment.EmpIdNullable)
                && !_edrsData.IsTemporary(employment.EmpIdNullable)
                && !HasValidChecksum(employment.EmpIdNullable.Value);

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