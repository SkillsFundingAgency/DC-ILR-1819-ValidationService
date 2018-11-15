using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_09Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ESMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "ESMType_09";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 07 (rule)
        /// </summary>
        private readonly IDD07 _derivedData07;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_09Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData07">The derived data 07.</param>
        public ESMType_09Rule(
            IValidationErrorHandler validationErrorHandler,
            IDD07 derivedData07)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));

            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public DateTime FirstViableDate => new DateTime(2013, 08, 01);

        /// <summary>
        /// Gets the lastest qualifying date.
        /// </summary>
        /// <param name="deliveries">The deliveries.</param>
        /// <returns>a date time for the latest qualifying learning aim</returns>
        public DateTime? GetLastestQualifyingDate(IReadOnlyCollection<ILearningDelivery> deliveries) =>
            deliveries
                .SafeWhere(IsACandidate)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault()?
                .LearnStartDate;

        /// <summary>
        /// Determines whether the specified delivery is a candidate.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is a candidate; otherwise, <c>false</c>.
        /// </returns>
        public bool IsACandidate(ILearningDelivery delivery) =>
            IsApprenticeship(delivery) && InAProgramme(delivery);

        /// <summary>
        /// Determines whether the specified delivery is apprenticeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether [in a programme] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in a programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        /// <summary>
        /// Determines whether [is qualifying period] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <param name="lastViableDate">The last viable date.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying period] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingPeriod(ILearnerEmploymentStatus employmentStatus, DateTime lastViableDate) =>
            It.IsBetween(employmentStatus.DateEmpStatApp, FirstViableDate, lastViableDate);

        /// <summary>
        /// Determines whether [is qualifying employment] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying employment] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            It.IsInRange(employmentStatus.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Determines whether [has qualifying indicator] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(monitor.ESMType, Monitoring.EmploymentStatus.Types.LengthOfEmployment);

        /// <summary>
        /// Determines whether [has qualifying indicator] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.SafeAny(HasQualifyingIndicator);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <param name="lastViabledate">The last viabledate.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus, DateTime? lastViabledate) =>
            It.Has(lastViabledate)
                && IsQualifyingPeriod(employmentStatus, lastViabledate.Value)
                && IsQualifyingEmployment(employmentStatus)
                && !HasQualifyingIndicator(employmentStatus);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var qualifyingDate = GetLastestQualifyingDate(objectToValidate.LearningDeliveries);

            objectToValidate.LearnerEmploymentStatuses
                .SafeWhere(x => IsNotValid(x, qualifyingDate))
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
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.EmploymentStatus.Types.LengthOfEmployment));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}