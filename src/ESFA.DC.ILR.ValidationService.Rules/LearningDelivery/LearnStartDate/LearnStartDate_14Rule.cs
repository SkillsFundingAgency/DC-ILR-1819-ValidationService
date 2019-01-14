using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnStartDate_14";

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The derived date (rule) 18
        /// </summary>
        private readonly IDerivedData_18Rule _derivedData18;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData18">The derived date18.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnStartDate_14Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_18Rule derivedData18,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, Name)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(derivedData18)
                .AsGuard<ArgumentNullException>(nameof(derivedData18));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _larsData = larsData;
            _derivedData18 = derivedData18;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the start for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>a start date for a standard apprecticehsip aim (or null for others)</returns>
        public DateTime? GetStartFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData18.GetApprenticeshipStandardProgrammeStartDateFor(thisDelivery, usingSources);

        /// <summary>
        /// Gets the periods of validity for (this delivery).
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a collection of validity periods</returns>
        public IReadOnlyCollection<ILARSStandardValidity> GetPeriodsOfValidityFor(ILearningDelivery thisDelivery) =>
            _larsData.GetStandardValiditiesFor(thisDelivery.StdCodeNullable.Value);

        /// <summary>
        /// Determines whether [has standard code] [the specified this delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has standard code] [the specified this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasStandardCode(ILearningDelivery thisDelivery) =>
            It.Has(thisDelivery.StdCodeNullable);

        /// <summary>
        /// Determines whether [has qualifying period of validity] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="periodsOfValidity">The periods of validity.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying period of validity] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingPeriodOfValidity(DateTime? candidate, IReadOnlyCollection<ILARSStandardValidity> periodsOfValidity) =>
            It.Has(candidate)
                && periodsOfValidity.Any(x => x.IsCurrent(candidate.Value));

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            !_check.IsRestart(delivery) // <= a singular exclusion clause
                && _check.IsStandardApprencticeship(delivery)
                && HasStandardCode(delivery)
                && !HasQualifyingPeriodOfValidity(GetStartFor(delivery, usingSources), GetPeriodsOfValidityFor(delivery));

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

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsNotValid(x, deliveries))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
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
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, thisDelivery.StdCodeNullable)
            };
        }
    }
}
