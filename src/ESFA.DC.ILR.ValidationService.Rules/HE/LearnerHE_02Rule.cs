using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnerHE_02Rule :
            IRule<ILearner>
    {
        private IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerHE_02Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public LearnerHE_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => RuleNameConstants.LearnerHE_02;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var learningDeliveries = objectToValidate.LearningDeliveries;

            var failedValidation = !ConditionMet(learnerHE, learningDeliveries);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, learnerHE);
            }
        }

        /// <summary>
        /// Conditions the met.
        /// </summary>
        /// <param name="learnerHE">The learner he.</param>
        /// <param name="learningDeliveries">The learning deliveries.</param>
        /// <returns>true if any any point the conditions are met</returns>
        public bool ConditionMet(ILearnerHE learnerHE, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return It.Has(learnerHE)
                ? It.HasValues(learningDeliveries) && learningDeliveries.Any(d => It.Has(d.LearningDeliveryHEEntity))
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="learnerHE">The learner HE.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerHE learnerHE)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(learnerHE), learnerHE));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
