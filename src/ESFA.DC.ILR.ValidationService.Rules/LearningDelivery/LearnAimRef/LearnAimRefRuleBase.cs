using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public abstract class LearnAimRefRuleBase :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The action provider
        /// </summary>
        private readonly IProvideLearnAimRefRuleActions _actionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleBase" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="ruleName">Name of the rule.</param>
        protected LearnAimRefRuleBase(
            IValidationErrorHandler validationErrorHandler,
            IProvideLearnAimRefRuleActions provider,
            ILARSDataService larsData,
            string ruleName)
            : base(validationErrorHandler, ruleName)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(provider)
                .AsGuard<ArgumentNullException>(nameof(provider));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));

            _actionProvider = provider;
            LarsData = larsData;
        }

        /// <summary>
        /// Gets the lars data (service)
        /// </summary>
        protected ILARSDataService LarsData { get; }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchCategory">The branch category.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public abstract bool PassesConditions(ILearningDelivery delivery, IBranchResult branchCategory);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            !PassesConditions(delivery, _actionProvider.GetBranchingResultFor(delivery, learner));

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
                .ForAny(x => IsNotValid(x, objectToValidate), x => RaiseValidationMessage(learnRefNumber, x));
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
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef)
            };
        }
    }
}
