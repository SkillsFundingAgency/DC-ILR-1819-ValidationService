using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
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
        /// The checks (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_18Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_18Rule(
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
        /// Gets the old code monitoring threshold date.
        /// </summary>
        public static DateTime OldCodeMonitoringThresholdDate => new DateTime(2018, 07, 31);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

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
                    .SafeWhere(x => x.DateEmpStatApp <= matchingDelivery.LearnStartDate)
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
            _check.IsTraineeship(delivery)
                && _check.InAProgramme(delivery)
                && _check.HasQualifyingStart(delivery, DateTime.MinValue, OldCodeMonitoringThresholdDate)
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
