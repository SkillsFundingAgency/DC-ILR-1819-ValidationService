using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the postcode data service implementation
    /// </summary>
    /// <seealso cref="AbstractDataRetrievalService" />
    /// <seealso cref="IPostcodesDataRetrievalService" />
    public class PostcodesDataRetrievalService :
        AbstractDataRetrievalService,
        IPostcodesDataRetrievalService
    {
        private readonly IPostcodes _postcodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostcodesDataRetrievalService"/> class.
        /// </summary>
        public PostcodesDataRetrievalService()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostcodesDataRetrievalService"/> class.
        /// </summary>
        /// <param name="postcodes">The postcodes.</param>
        /// <param name="messageCache">The message cache.</param>
        public PostcodesDataRetrievalService(IPostcodes postcodes, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _postcodes = postcodes;
        }

        public async Task<IEnumerable<string>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var matchingPostcodes = new List<string>();

            var postcodeShards = UniquePostcodesFromMessage(_messageCache.Item)
                .SplitList(5000)
                .Select(p => p.ToCaseInsensitiveHashSet());

            foreach (var shard in postcodeShards)
            {
                matchingPostcodes.AddRange(
                    await _postcodes.MasterPostcodes
                .Where(p => shard.Contains(p.Postcode))
                .Select(p => p.Postcode)
                .ToListAsync(cancellationToken));
            }

            return matchingPostcodes;
        }

        /// <summary>
        /// Retrieves the ons postcodes asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>a set of ons postcode records</returns>
        public async Task<IReadOnlyCollection<IONSPostcode>> RetrieveONSPostcodesAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(
                () =>
                {
                    var message = _messageCache.Item;
                    It.IsNull(message)
                        .AsGuard<ArgumentNullException>(nameof(message));

                    var deliveries = message.Learners
                            .AsSafeReadOnlyList()
                            .SelectMany(x => x.LearningDeliveries.AsSafeReadOnlyList())
                            .AsSafeReadOnlyList();
                    var uniquePostcodes = GetUniqueDeliveryLocationPostcodesFrom(deliveries).ToCaseInsensitiveHashSet();

                    return _postcodes.ONS_Postcodes
                        .Where(p => uniquePostcodes.Contains(p.Postcode))
                        .Select(p => new ONSPostcode
                        {
                            Postcode = p.Postcode,
                            EffectiveFrom = p.EffectiveFrom,
                            EffectiveTo = p.EffectiveTo,
                            LocalAuthority = p.LocalAuthority,
                            Termination = GetDateFromYearMonthString(p.Termination)
                        })
                        .AsSafeReadOnlyList();
                }, cancellationToken);
        }

        public IEnumerable<string> UniquePostcodesFromMessage(IMessage message)
        {
            return UniqueLearnerPostcodesFromMessage(message)
                .Union(UniqueLearnerPostcodePriorsFromMessage(message))
                .Union(UniqueLearningDeliveryLocationPostcodesFromMessage(message))
                .Distinct();
        }

        public virtual IEnumerable<string> UniqueLearnerPostcodesFromMessage(IMessage message)
        {
            return message?
                        .Learners?
                        .Where(l => l.Postcode != null)
                        .Select(l => l.Postcode)
                        .Distinct()
                    ?? new List<string>();
        }

        /// <summary>
        /// Gets the unique delivery location postcodes from.
        /// </summary>
        /// <param name="deliveries">The deliveries.</param>
        /// <returns>a set of delivery location postcodes</returns>
        public virtual IReadOnlyCollection<string> GetUniqueDeliveryLocationPostcodesFrom(IReadOnlyCollection<ILearningDelivery> deliveries)
        {
            return deliveries
                .SafeWhere(x => It.Has(x.DelLocPostCode))
                .Select(x => x.DelLocPostCode)
                .Distinct()
                .AsSafeReadOnlyList();
        }

        public virtual IEnumerable<string> UniqueLearnerPostcodePriorsFromMessage(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.PostcodePrior != null)
                       .Select(l => l.PostcodePrior)
                       .Distinct()
                   ?? new List<string>();
        }

        public virtual IEnumerable<string> UniqueLearningDeliveryLocationPostcodesFromMessage(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Select(ld => ld.DelLocPostCode)
                       .Distinct()
                   ?? new List<string>();
        }

        private DateTime? GetDateFromYearMonthString(string yearMonth)
        {
            if (string.IsNullOrEmpty(yearMonth.Trim()))
            {
                return null;
            }

            if (yearMonth.Length < 6)
            {
                return null;
            }

            var yearParsed = int.TryParse(yearMonth.Substring(0, 4), out var year);
            var monthParsed = int.TryParse(yearMonth.Substring(4), out var month);

            if (!yearParsed || !monthParsed)
            {
                return null;
            }

            return new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        }
    }
}
