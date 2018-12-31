﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_12Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ESMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "ESMType_12";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The checker (common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_12Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations provider.</param>
        public ESMType_12Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _messageHandler = validationErrorHandler;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2013, 08, 01);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is qualifying employment] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying employment] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            It.IsInRange(
                employmentStatus.EmpStat,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);

        /// <summary>
        /// Determines whether [has disqualifying indicator] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                monitor.ESMType,
                Monitoring.EmploymentStatus.Types.SelfEmploymentIndicator,
                Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator);

        /// <summary>
        /// Determines whether [has disqualifying indicator] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.SafeAny(HasDisqualifyingIndicator);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            _check.HasQualifyingStart(employmentStatus, FirstViableDate)
                && IsQualifyingEmployment(employmentStatus)
                && HasDisqualifyingIndicator(employmentStatus);

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
        /// <param name="thisEmployment">The this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, "Invalid"));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}