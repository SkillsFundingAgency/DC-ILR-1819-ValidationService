using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class AFinType_13Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "AFINDATE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "AFinType_13";

        /// <summary>
        /// The apprenticeships (fund model)
        /// </summary>
        public const int ApprenticeshipsFundModel = 36;

        /// <summary>
        /// The programme aim (type)
        /// </summary>
        public const int ProgrammeAim = 1;

        /// <summary>
        /// The total negotiated price (apprenticeship financial type)
        /// </summary>
        public const string TotalNegotiatedPrice = "TNP";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFinType_13Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public AFinType_13Rule(IValidationErrorHandler validationErrorHandler)
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

            objectToValidate.LearningDeliveries?
                .Where(d => d.FundModel == ApprenticeshipsFundModel && d.AimType == ProgrammeAim)
                .ForEach(x =>
                {
                    var finRecords = x.AppFinRecords?
                        .Where(afr => afr.AFinType == TotalNegotiatedPrice)
                        .AsSafeReadOnlyList();

                    /* candidate change - under discussion with mark / sanjeev
                    var failedValidation = !finRecords.Any(y => ConditionMet(x, y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x, y);
                    }
                    */

                    finRecords.ForEach(y =>
                    {
                        var failedValidation = !ConditionMet(x, y);

                        if (failedValidation)
                        {
                            RaiseValidationMessage(learnRefNumber, x, y);
                        }
                    });
                });
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="thisDelivery">this learning delivery.</param>
        /// <param name="thisFinancialRecord">this financial record.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDelivery thisDelivery, IAppFinRecord thisFinancialRecord)
        {
            return It.Has(thisDelivery)
                ? It.Has(thisFinancialRecord)
                    && thisFinancialRecord.AFinDate > DateTime.MinValue
                    && thisDelivery.LearnStartDate == thisFinancialRecord.AFinDate
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this learning delivery.</param>
        /// <param name="thisFinancialRecord">this financial record.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IAppFinRecord thisFinancialRecord)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisFinancialRecord));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
