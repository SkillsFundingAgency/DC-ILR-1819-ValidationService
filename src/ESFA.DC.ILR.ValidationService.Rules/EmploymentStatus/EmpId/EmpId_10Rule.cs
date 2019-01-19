using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_10Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_10Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData07">The derived data 07.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpId_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpId_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the lastest qualifying date.
        /// </summary>
        /// <param name="deliveries">The deliveries.</param>
        /// <returns>a date time for the latest qualifying learning aim</returns>
        public DateTime? GetQualifyingDate(IReadOnlyCollection<ILearningDelivery> deliveries) =>
            deliveries
                .Where(IsACandidate)
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
           _check.InApprenticeship(delivery)
                && _check.InAProgramme(delivery);

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
        /// Determines whether [is qualifying employment date] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <param name="qualifyingDate">The qualifying date.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying employment date] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingEmploymentDate(ILearnerEmploymentStatus employmentStatus, DateTime qualifyingDate) =>
            employmentStatus.DateEmpStatApp <= qualifyingDate;

        /// <summary>
        /// Determines whether [has qualifying employer] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employer] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmployer(ILearnerEmploymentStatus employmentStatus) =>
            It.Has(employmentStatus.EmpIdNullable);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <param name="qualifyingDate">The qualifying date.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus, DateTime qualifyingDate) =>
                IsQualifyingEmploymentDate(employmentStatus, qualifyingDate)
                && IsQualifyingEmployment(employmentStatus)
                && !HasQualifyingEmployer(employmentStatus);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var deliveries = objectToValidate.LearningDeliveries.AsSafeReadOnlyList();
            var qualifyingDate = GetQualifyingDate(deliveries);

            if (It.Has(qualifyingDate))
            {
                objectToValidate.LearnerEmploymentStatuses
                    .SafeWhere(x => IsNotValid(x, qualifyingDate.Value))
                    .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisEmployment">The this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            HandleValidationError(learnRefNumber, null, BuildParametersFor(thisEmployment));
        }

        /// <summary>
        /// Builds the parameters for (this employment)
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>a set of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildParametersFor(ILearnerEmploymentStatus thisEmployment)
        {
            return new IErrorMessageParameter[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, "(missing)"),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, thisEmployment.EmpStat),
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, thisEmployment.DateEmpStatApp)
            };
        }
    }
}