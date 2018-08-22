using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class ProgType_03Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ProgType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "ProgType_03";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgType_03Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public ProgType_03Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learningDeliveries = objectToValidate.LearningDeliveries;

            var failedValidation = !ConditionMet(learningDeliveries);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, learningDeliveries);
            }
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="theseDeliveries">these deliveries.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(IReadOnlyCollection<ILearningDelivery> theseDeliveries)
        {
            return It.HasValues(theseDeliveries)
                ? theseDeliveries
                    .Where(x => It.Has(x.ProgTypeNullable))
                    .All(x => TypeOfProgramme.AsASet.Contains((int)x.ProgTypeNullable))
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theseDeliveries">these deliveries.</param>
        public void RaiseValidationMessage(string learnRefNumber, IReadOnlyCollection<ILearningDelivery> theseDeliveries)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, theseDeliveries));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
