using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_04Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpStat_04";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_04Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 07 rule.</param>
        public EmpStat_04Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));

            _messageHandler = validationErrorHandler;
            _derivedData22 = derivedData22;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the latest learning start for esf contract.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="sources">The sources.</param>
        /// <returns>a date time of the latest completed ESF contract matching the contract reference number, or null</returns>
        public DateTime? GetLatestLearningStartForESFContract(
            ILearningDelivery delivery,
            IReadOnlyCollection<ILearningDelivery> sources) =>
                _derivedData22.GetLatestLearningStartForESFContract(delivery, sources);

        /// <summary>
        /// Determines whether [is not known or provided] [the specified status].
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        ///   <c>true</c> if [is not known or provided] [the specified status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotKnownOrProvided(ILearnerEmploymentStatus status) =>
            It.IsInRange(status.EmpStat, TypeOfEmploymentStatus.NotKnownProvided);

        /// <summary>
        /// Determines whether [has qualifying completed contract] [this learner].
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        /// <param name="matchingThisDate">matching this date.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying completed contract] [this learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCompletedContract(ILearner thisLearner, DateTime matchingThisDate)
        {
            var deliveries = thisLearner.LearningDeliveries.SafeWhere(x => x.LearnStartDate == matchingThisDate);

            return deliveries.SafeAny(x => GetLatestLearningStartForESFContract(x, thisLearner.LearningDeliveries) == matchingThisDate);
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="status">The status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearner learner, ILearnerEmploymentStatus status) =>
            IsNotKnownOrProvided(status)
            && HasQualifyingCompletedContract(learner, status.DateEmpStatApp);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            objectToValidate.LearnerEmploymentStatuses
                .SafeWhere(x => IsNotValid(objectToValidate, x))
                .ForEach(x => RaiseValidationMessage(objectToValidate));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learner">The learner.</param>
        public void RaiseValidationMessage(ILearner learner)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, TypeOfEmploymentStatus.NotKnownProvided));

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, null, parameters);
        }
    }
}
