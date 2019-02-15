using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookupDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_06Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookupDetails">The lookup details.</param>
        public LearnDelFAMType_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));

            _lookupDetails = lookupDetails;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="thisLearner">The object to validate.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            thisLearner.LearningDeliveries
                .ForEach(x => CheckDeliveryFAMs(x, y => RaiseValidationMessage(thisLearner.LearnRefNumber, x, y)));
        }

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="learningDelivery">The learning delivery.</param>
        /// <param name="raiseMessage">The raise message.</param>
        public void CheckDeliveryFAMs(ILearningDelivery learningDelivery, Action<ILearningDeliveryFAM> raiseMessage)
        {
            learningDelivery.LearningDeliveryFAMs
                .ForAny(x => IsNotCurrent(x, learningDelivery.LearnStartDate), raiseMessage);
        }

        /// <summary>
        /// Determines whether [is not current] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if [is not current] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotCurrent(ILearningDeliveryFAM monitor, DateTime referenceDate)
        {
            return !_lookupDetails.IsCurrent(
                TypeOfLimitedLifeLookup.LearningDeliveryFAM,
                $"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}",
                referenceDate);
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="andMonitor">The and monitor.</param>
        private void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, ILearningDeliveryFAM andMonitor)
        {
            var parameters = new IErrorMessageParameter[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, andMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, andMonitor.LearnDelFAMCode)
            };

            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
