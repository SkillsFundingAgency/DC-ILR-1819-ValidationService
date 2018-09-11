using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class PriorLearnFundAdj_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "PriorLearnFundAdj";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "PriorLearnFundAdj_02";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorLearnFundAdj_02Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public PriorLearnFundAdj_02Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [is right fund model] [for the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is right fund model] [for the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightFundModel(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills);

        /// <summary>
        /// Determines whether [is component aim] [for the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component aim] [for the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentAim(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ComponentAimInAProgramme, TypeOfAim.AimNotPartOfAProgramme);

        /// <summary>
        /// Determines whether [is a restart] [for the specified delivery].
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>
        ///   <c>true</c> if [is a restart] [for the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams?.Any(x => It.IsInRange(x.LearnDelFAMType, LearningDeliveryFAMTypeConstants.RES)) ?? false;

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
                .Where(x => IsRightFundModel(x) && IsComponentAim(x) && IsRestart(x.LearningDeliveryFAMs))
                .ForEach(x =>
                {
                    var failedValidation = !ConditionMet(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return It.Has(thisDelivery)
                ? It.IsEmpty(thisDelivery.PriorLearnFundAdjNullable)
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
