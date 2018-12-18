using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_18Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ESMType-Code";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpStat_18";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_18Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="yearData">The year data.</param>
        public EmpStat_18Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the old code monitoring threshold date.
        /// </summary>
        /// <value>
        /// The old code monitoring threshold date.
        /// </value>
        public DateTime OldCodeMonitoringThresholdDate => new DateTime(2018, 08, 01);

        /// <summary>
        /// Determines whether [in training] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

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
        /// Determines whether [has qualifying start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate < OldCodeMonitoringThresholdDate;

        /// <summary>
        /// Determines whether [has a qualifying monitor status] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying monitor status] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingMonitorStatus(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange($"{monitor.ESMType}{monitor.ESMCode}", Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW);

        /// <summary>
        /// Checks the employment monitors.
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if the match condition is met</returns>
        public bool CheckEmploymentMonitors(ILearnerEmploymentStatus employment, Func<IEmploymentStatusMonitoring, bool> matchCondition) =>
            employment.EmploymentStatusMonitorings.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment].
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employment) =>
            !CheckEmploymentMonitors(employment, HasAQualifyingMonitorStatus);

        /// <summary>
        /// Determines whether [does not have a qualifying employment status] [using the specified employments].
        /// </summary>
        /// <param name="usingEmployments">using employments.</param>
        /// <param name="matchingDelivery">matching delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying employment status] [using the specified employments]; otherwise, <c>false</c>.
        /// </returns>
        public bool DoesNotHaveAQualifyingEmploymentStatus(
            IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments,
            ILearningDelivery matchingDelivery) =>
                It.IsEmpty(usingEmployments)
                || usingEmployments
                    .SafeWhere(x => x.DateEmpStatApp == matchingDelivery.LearnStartDate)
                    .Any(IsNotValid);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingEmployments">The using employments.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            InTraining(delivery)
                && InAProgramme(delivery)
                && HasQualifyingStart(delivery)
                && DoesNotHaveAQualifyingEmploymentStatus(usingEmployments, delivery);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var employments = objectToValidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

            objectToValidate.LearningDeliveries
                .Where(x => IsNotValid(x, employments))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">The this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
