using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    /// <summary>
    /// apprenticeship record financial date
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class AFinDate_03Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "AFINDATE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "AFinDate_03";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The file data (service)
        /// </summary>
        private readonly IFileDataService _fileData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFinDate_03Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileData">The file data.</param>
        public AFinDate_03Rule(IValidationErrorHandler validationErrorHandler, IFileDataService fileData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));

            _messageHandler = validationErrorHandler;
            _fileData = fileData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Checks the delivery apprecticeship fianancial records.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryAFRs(ILearningDelivery delivery, Func<IAppFinRecord, bool> matchCondition) =>
            delivery.AppFinRecords.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [has invalid financial date] [the specified record].
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>
        ///   <c>true</c> if [has invalid financial date] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasInvalidFinancialDate(IAppFinRecord record) =>
            record.AFinDate > _fileData.FilePreparationDate();

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
                .SafeWhere(d => CheckDeliveryAFRs(d, HasInvalidFinancialDate))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
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
