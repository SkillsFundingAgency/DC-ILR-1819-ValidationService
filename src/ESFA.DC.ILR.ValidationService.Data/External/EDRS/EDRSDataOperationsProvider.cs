using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.EDRS
{
    /// <summary>
    /// i provide EDRS data operations implementation
    /// </summary>
    /// <seealso cref="IProvideEDRSDataOperations" />
    public class EDRSDataOperationsProvider :
        IProvideEDRSDataOperations
    {
        /// <summary>
        /// The temporary identifier
        /// </summary>
        public const int TemporaryID = 999999999;

        /// <summary>
        /// The employer reference numbers
        /// </summary>
        private readonly IReadOnlyCollection<int> _employers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EDRSDataOperationsProvider"/> class.
        /// </summary>
        /// <param name="referenceDataCache">The reference data cache.</param>
        public EDRSDataOperationsProvider(IExternalDataCache referenceDataCache)
        {
            _employers = referenceDataCache.ERNs.AsSafeReadOnlyList();
        }

        /// <summary>
        /// Determines whether the specified this employer is temporary.
        /// </summary>
        /// <param name="thisEmployer">this employer (reference number).</param>
        /// <returns>
        /// <c>true</c> if the specified this employer is temporary; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTemporary(int thisEmployer)
        {
            return It.IsInRange(thisEmployer, TemporaryID);
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="thisEmployer">this employer (reference number).</param>
        /// <returns>
        /// <c>true</c> if the specified this employer is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(int thisEmployer)
        {
            return IsTemporary(thisEmployer)
                || _employers.Contains(thisEmployer);
        }
    }
}
