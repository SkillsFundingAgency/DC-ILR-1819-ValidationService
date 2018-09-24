using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.SWSSupAimId
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class SWSSupAimId_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "SWSSupAimId";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "SWSSupAimId_01";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SWSSupAimId_01Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public SWSSupAimId_01Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [is valid unique identifier] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is valid unique identifier] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidGuid(string candidate) =>
            Guid.TryParse(candidate, out Guid result) && It.IsUsable(result);

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
                .Where(x => It.Has(x.SWSupAimId))
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
                ? IsValidGuid(thisDelivery.SWSupAimId)
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
