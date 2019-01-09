﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_05Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpStat_05";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookupDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_05Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookupDetails">The lookup details.</param>
        public EmpStat_05Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));

            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is not valid] [the specified status].
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus status) =>
            !_lookupDetails.Contains(LookupSimpleKey.EmpStat, status.EmpStat);

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
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="status">The status.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus status)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, status.EmpStat));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
