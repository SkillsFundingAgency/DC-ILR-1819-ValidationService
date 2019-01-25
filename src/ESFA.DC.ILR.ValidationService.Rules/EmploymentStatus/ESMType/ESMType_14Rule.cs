﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checker (common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations provider.</param>
        public ESMType_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.ESMType_14)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2013, 08, 01);

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
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisEmployment">The this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisEmployment));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearnerEmploymentStatus thisEmployment, IEmploymentStatusMonitoring thisMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, thisMonitor.ESMType),
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, thisEmployment.DateEmpStatApp),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, thisEmployment.EmpStat)
            };
        }
    }
}