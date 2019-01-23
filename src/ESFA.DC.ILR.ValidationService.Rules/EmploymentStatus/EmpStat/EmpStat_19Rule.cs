using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_19Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checks (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_19Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_19Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_19)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the old code monitoring threshold date.
        /// </summary>
        public static DateTime NewCodeMonitoringThresholdDate => new DateTime(2018, 08, 01);

        /// <summary>
        /// Determines whether [has a qualifying monitor status] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying monitor status] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasADisqualifyingMonitorStatus(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.EmployedFor0To10HourPW,
                Monitoring.EmploymentStatus.EmployedFor11To20HoursPW);

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
            !CheckEmploymentMonitors(employment, HasADisqualifyingMonitorStatus);

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
                && _check.HasQualifyingStart(delivery, NewCodeMonitoringThresholdDate)
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
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, thisDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, thisDelivery.StdCodeNullable)
            };
        }
    }
}
