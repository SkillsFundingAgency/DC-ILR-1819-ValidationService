using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_37Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnAimRef";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnAimRef_37";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_37Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        public LearnAimRef_37Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the minimun viable start.
        /// </summary>
        public DateTime MinimumViableStart => new DateTime(2011, 07, 31);

        /// <summary>
        /// Determines whether [is apprenticeship funded] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is apprenticeship funded] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipFunded(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills) && It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.ApprenticeshipStandard);

        /// <summary>
        /// Determines whether [is other funded] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is other funded] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOtherFunded(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.OtherAdult, TypeOfFunding.NotFundedByESFA);

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > MinimumViableStart;

        /// <summary>
        /// Determines whether [has valid learning aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has valid learning aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidLearningAim(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef).AsSafeReadOnlyList();

            return validities
                .Where(x => IsCurrent(x, delivery))
                .Any(HasQualifyingCategory);
        }

        /// <summary>
        /// Determines whether the specified validity is current.
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified validity is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(ILARSValidity validity, ILearningDelivery delivery) =>
            It.IsBetween(delivery.LearnStartDate, validity.StartDate, validity.EndDate ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether [has qualifying category] [the specified validity].
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying category] [the specified validity]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCategory(ILARSValidity validity) =>
            It.IsInRange(validity.ValidityCategory, TypeOfLARSValidity.Any);

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [is advanced learner loan] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdvancedLearnerLoan(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, DeliveryMonitoring.Types.AdvancedLearnerLoan);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsViableStart(x) && (IsApprenticeshipFunded(x) || IsOtherFunded(x)) && !CheckDeliveryFAMs(x, IsAdvancedLearnerLoan))
                .ForEach(x =>
                {
                    var failedValidation = !HasValidLearningAim(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
