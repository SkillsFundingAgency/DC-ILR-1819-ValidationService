using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_09Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnDelFAMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnDelFAMType_09";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_09Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public LearnDelFAMType_09Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [has esfa adultfunding] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has esfa adultfunding] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasESFAAdultFunding(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ESFAAdultFunding);

        /// <summary>
        /// Determines whether [has esfa adultfunding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has esfa adultfunding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasESFAAdultFunding(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, HasESFAAdultFunding);

        /// <summary>
        /// Determines whether [is qualifying funding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingFundModel(ILearningDelivery delivery) =>
            It.IsInRange(
                delivery.FundModel,
                TypeOfFunding.CommunityLearning,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult);

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
                .SafeWhere(IsQualifyingFundModel)
                .ForEach(x =>
                {
                    var failedValidation = !HasESFAAdultFunding(x);

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
