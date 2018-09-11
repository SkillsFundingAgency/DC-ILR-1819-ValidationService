using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_66Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnDelFAMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnDelFAMType_66";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_66Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public LearnDelFAMType_66Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Gets the minimun viable start.
        /// </summary>
        public DateTime MinimunViableStart => new DateTime(2017, 07, 31);

        /// <summary>
        /// Gets the minimum viable age.
        /// </summary>
        public TimeSpan MinimumViableAge => new TimeSpan(8765, 0, 0, 0);

        /// <summary>
        /// Determines whether the specified delivery is funded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is funded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFunding(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills);

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > MinimunViableStart;

        /// <summary>
        /// Determines whether [is viable age] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable age] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableAge(ILearner learner, ILearningDelivery delivery) =>
            It.Has(learner.DateOfBirthNullable) && ((delivery.LearnStartDate - learner.DateOfBirthNullable) > MinimumViableAge);

        /// <summary>
        /// Determines whether [is low earner] [the specified fam].
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>
        ///   <c>true</c> if [is low earner] [the specified fam]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLowEarner(ILearningDeliveryFAM fam) =>
            It.IsInRange(fam.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM) && It.IsInRange(fam.LearnDelFAMCode, DeliveryMonitoring.InReceiptOfLowWages);

        /// <summary>
        /// Determines whether [is steel worker] [the specified fam].
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker] [the specified fam]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSteelWorker(ILearningDeliveryFAM fam) =>
            It.IsInRange(fam.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM) && It.IsInRange(fam.LearnDelFAMCode, DeliveryMonitoring.SteelIndustriesRedundancyTraining);

        public bool IsUnemployed(ILearningDeliveryFAM fam) =>
            It.IsInRange(fam.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM) && It.IsInRange(fam.LearnDelFAMCode, DeliveryMonitoring.SteelIndustriesRedundancyTraining);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries?
                .Where(x => IsAdultFunding(x) && IsViableStart(x) && IsViableAge(objectToValidate, x))
                .ForEach(x =>
                {
                    var famRecords = x.LearningDeliveryFAMs.AsSafeReadOnlyList();
                    var failedValidation = !famRecords.Any(y => ConditionMet(y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="famRecord">The (learning delivery) fam record.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDeliveryFAM famRecord)
        {
            return It.Has(famRecord)
                ? IsLowEarner(famRecord) || IsSteelWorker(famRecord) || It.IsInRange(famRecord.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF)
                : true;
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
