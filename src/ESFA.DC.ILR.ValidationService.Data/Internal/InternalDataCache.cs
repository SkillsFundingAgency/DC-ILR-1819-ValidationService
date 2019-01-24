using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal
{
    public class InternalDataCache : IInternalDataCache
    {
        /// <summary>
        /// The simple lookups
        /// </summary>
        private Dictionary<LookupSimpleKey, IReadOnlyCollection<int>> _simpleLookups;

        /// <summary>
        /// The coded lookups
        /// </summary>
        private Dictionary<LookupCodedKey, IReadOnlyCollection<string>> _codedLookups;

        /// <summary>
        /// The coded lookups
        /// </summary>
        private Dictionary<LookupCodedKeyDictionary, IDictionary<string, IReadOnlyCollection<string>>> _codedDictionaryLookups;

        private Dictionary<LookupComplexKey, IDictionary<string, IDictionary<string, ValidityPeriods>>> _codedComplexLookups;

        /// <summary>
        /// The time restricted lookups
        /// </summary>
        private Dictionary<LookupTimeRestrictedKey, IDictionary<string, ValidityPeriods>> _limitedLifeLookups;

        private Dictionary<LookupItemKey, IDictionary<string, IReadOnlyCollection<string>>> _itemLookups;

        public IAcademicYear AcademicYear { get; set; }

        /// <summary>
        /// Gets the simple lookups.
        /// </summary>
        public IDictionary<LookupSimpleKey, IReadOnlyCollection<int>> SimpleLookups
        {
            get
            {
                return _simpleLookups
                  ?? (_simpleLookups = new Dictionary<LookupSimpleKey, IReadOnlyCollection<int>>());
            }
        }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        public IDictionary<LookupCodedKey, IReadOnlyCollection<string>> CodedLookups
        {
            get
            {
                return _codedLookups
                  ?? (_codedLookups = new Dictionary<LookupCodedKey, IReadOnlyCollection<string>>());
            }
        }

        /// <summary>
        /// Gets the complex coded lookups.
        /// </summary>
        public IDictionary<LookupCodedKeyDictionary, IDictionary<string, IReadOnlyCollection<string>>> CodedDictionaryLookups
        {
            get
            {
                return _codedDictionaryLookups
                  ?? (_codedDictionaryLookups = new Dictionary<LookupCodedKeyDictionary, IDictionary<string, IReadOnlyCollection<string>>>());
            }
        }

        public IDictionary<LookupItemKey, IDictionary<string, IReadOnlyCollection<string>>> ItemLookups
        {
            get
            {
                return _itemLookups
                       ?? (_itemLookups = new Dictionary<LookupItemKey, IDictionary<string, IReadOnlyCollection<string>>>());
            }
        }

        /// <summary>
        /// Gets the time limited lookups.
        /// </summary>
        public IDictionary<LookupTimeRestrictedKey, IDictionary<string, ValidityPeriods>> LimitedLifeLookups
        {
            get
            {
                return _limitedLifeLookups
                  ?? (_limitedLifeLookups = new Dictionary<LookupTimeRestrictedKey, IDictionary<string, ValidityPeriods>>());
            }
        }

        public IDictionary<LookupComplexKey, IDictionary<string, IDictionary<string, ValidityPeriods>>> CodedComplexLookups
        {
            get
            {
                return _codedComplexLookups
                       ?? (_codedComplexLookups = new Dictionary<LookupComplexKey, IDictionary<string, IDictionary<string, ValidityPeriods>>>());
            }
        }
    }
}
