using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_16Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The fcs contract data provider
        /// </summary>
        private readonly IFCSDataService _contracts;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_16Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fcsData">The FCS data provider.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnStartDate_16Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_16)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _contracts = fcsData;
            _check = commonOperations;
        }

        public IReadOnlyCollection<IFcsContractAllocation> GetAllocationsFor(ILearningDelivery thisDelivery) =>
            _contracts.GetContractAllocationsFor(thisDelivery?.ConRefNumber);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="allocations">The allocations.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery thisDelivery, IReadOnlyCollection<IFcsContractAllocation> allocations) =>
            allocations.SafeAny(x => It.Has(x.StartDate) && _check.HasQualifyingStart(thisDelivery, x.StartDate.Value));

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            !HasQualifyingStart(thisDelivery, GetAllocationsFor(thisDelivery));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var deliveries = objectToValidate.LearningDeliveries.AsSafeReadOnlyList();

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsNotValid(x, deliveries))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
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
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
