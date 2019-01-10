using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class EPAOrgID_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "EPAOrgID";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EPAOrgID_02";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The epa provider
        /// </summary>
        private readonly IProvideEPAOrganisationDetails _ePAprovider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPAOrgID_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="ePAprovider">The epa provider.</param>
        public EPAOrgID_02Rule(IValidationErrorHandler validationErrorHandler, IProvideEPAOrganisationDetails ePAprovider)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(ePAprovider)
                .AsGuard<ArgumentNullException>(nameof(ePAprovider));

            _messageHandler = validationErrorHandler;
            _ePAprovider = ePAprovider;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is assessment price] [the specified record].
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>
        ///   <c>true</c> if [is assessment price] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAssessmentPrice(IAppFinRecord record) =>
            It.IsInRange($"{record.AFinType}{record.AFinCode}", ApprenticeshipFinancialRecord.TotalAssessmentPrice, ApprenticeshipFinancialRecord.ResidualAssessmentPrice);

        /// <summary>
        /// Determines whether [has assessment price] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has assessment price] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAssessmentPrice(ILearningDelivery delivery) =>
            delivery.AppFinRecords.SafeAny(IsAssessmentPrice);

        /// <summary>
        /// Determines whether the specified candidate identifier is known.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate identifier is known; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKnown(string candidateID) =>
            _ePAprovider.IsKnown(candidateID);

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
                .SafeWhere(HasAssessmentPrice)
                .ForEach(x =>
                {
                    var failedValidation = !IsKnown(x.EPAOrgID);

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
        /// <param name="thisDelivery">this learning delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
