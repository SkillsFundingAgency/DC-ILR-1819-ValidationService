using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    /// <summary>
    /// apprenticeship record financial date
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class AFinDate_03Rule :
        AbstractRule,
        IRule<ILearner>
    {
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
            : base(validationErrorHandler, RuleNameConstants.AFinDate_03)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));

            _fileData = fileData;
        }

        /// <summary>
        /// Checks the delivery apprecticeship fianancial records.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <param name="messageAction">The message action.</param>
        public void CheckDeliveryAFRs(ILearningDelivery delivery, Func<IAppFinRecord, bool> matchCondition, Action<IAppFinRecord> messageAction) =>
            delivery.AppFinRecords.ForAny(matchCondition, messageAction);

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
                .ForEach(x => CheckDeliveryAFRs(x, HasInvalidFinancialDate, y => RaiseValidationMessage(learnRefNumber, x, y)));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this learning delivery.</param>
        /// <param name="thisRecord">this financial record.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IAppFinRecord thisRecord)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisRecord));
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisRecord">this financial record.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(IAppFinRecord thisRecord)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, thisRecord.AFinDate),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, _fileData.FilePreparationDate())
            };
        }
    }
}
