using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// cross record rule 68
    /// There must not be more than one Apprenticeship Financial Record with
    /// the same combination of type, code and date across all apprenticeship
    /// standard programme aims for a given apprenticeship standard.
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class R68Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "R68";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="R68Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="message">The message.</param>
        public R68Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [has wrong code cardinality] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="allDeliveries">All deliveries.</param>
        /// <returns>
        ///   <c>true</c> if [has wrong code cardinality] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasWrongCodeCardinality(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> allDeliveries) =>
            It.Has(delivery.StdCodeNullable)
                && allDeliveries.SafeCount(x => It.Has(x.StdCodeNullable) && x.StdCodeNullable == delivery.StdCodeNullable) != 1;

        /// <summary>
        /// Gets the candidate codes.
        /// </summary>
        /// <param name="allDeliveries">All deliveries.</param>
        /// <returns>a list of qualifying standard codes</returns>
        public int[] GetCandidateCodes(IReadOnlyCollection<ILearningDelivery> allDeliveries) =>
            allDeliveries
                .SafeWhere(x => HasWrongCodeCardinality(x, allDeliveries))
                .Select(x => x.StdCodeNullable.Value)
                .Distinct()
                .ToArray();

        /// <summary>
        /// Determines whether [is qualifying item] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="standardCodes">The standard codes.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying item] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingItem(ILearningDelivery delivery, IReadOnlyCollection<int> standardCodes) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                && delivery.AimType == TypeOfAim.ProgrammeAim
                && It.IsInRange(delivery.StdCodeNullable, standardCodes.ToArray());

        /// <summary>
        /// Gets the flattened records.
        /// </summary>
        /// <param name="allDeliveries">All deliveries.</param>
        /// <param name="standardCodes">The standard codes.</param>
        /// <returns>a list of qualifying app fin records</returns>
        public IReadOnlyCollection<IAppFinRecord> GetFlattenedRecords(
            IReadOnlyCollection<ILearningDelivery> allDeliveries,
            IReadOnlyCollection<int> standardCodes) =>
                allDeliveries
                    .SafeWhere(x => IsQualifyingItem(x, standardCodes))
                    .SelectMany(x => x.AppFinRecords.AsSafeReadOnlyList())
                    .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has wrong record cardinality] [the specified record].
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="allRecords">All records.</param>
        /// <returns>
        ///   <c>true</c> if [has wrong record cardinality] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasWrongRecordCardinality(IAppFinRecord record, IReadOnlyCollection<IAppFinRecord> allRecords) =>
            allRecords.SafeCount(x => RecordEqualityComparer.Equals(x, record)) != 1;

        /// <summary>
        /// Gets the candidate records.
        /// </summary>
        /// <param name="flattenedRecords">The flattened records.</param>
        /// <returns>a distinct list of offenders</returns>
        public IReadOnlyCollection<IAppFinRecord> GetCandidateRecords(IReadOnlyCollection<IAppFinRecord> flattenedRecords) =>
            flattenedRecords
                .SafeWhere(x => HasWrongRecordCardinality(x, flattenedRecords))
                .Distinct(new RecordEqualityComparer())
                .AsSafeReadOnlyList();

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var deliveries = objectToValidate.LearningDeliveries
                .AsSafeReadOnlyList();
            var standardCodes = GetCandidateCodes(deliveries);
            var flattenedFinRecords = GetFlattenedRecords(deliveries, standardCodes);
            var candidates = GetCandidateRecords(flattenedFinRecords);

            candidates.ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="record">The record.</param>
        public void RaiseValidationMessage(string learnRefNumber, IAppFinRecord record)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfLearningProgramme.ApprenticeshipStandard));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinType, record.AFinType));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinCode, record.AFinCode));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinDate, record.AFinDate));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }

        /// <summary>
        /// an app fin record equality comparer, used in the rule
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEqualityComparer{IAppFinRecord}" />
        private class RecordEqualityComparer :
            IEqualityComparer<IAppFinRecord>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
            /// <returns>
            ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
            /// </returns>
            public static bool Equals(IAppFinRecord x, IAppFinRecord y)
            {
                return x.AFinCode == y.AFinCode
                    && x.AFinDate == y.AFinDate
                    && x.AFinType == y.AFinType;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
            /// <returns>
            ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
            /// </returns>
            bool IEqualityComparer<IAppFinRecord>.Equals(IAppFinRecord x, IAppFinRecord y)
            {
                return Equals(x, y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public int GetHashCode(IAppFinRecord obj)
            {
                return Tuple.Create(obj.AFinCode, obj.AFinDate, obj.AFinType).GetHashCode();
            }
        }
    }
}