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
        private Dictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> _simpleLookups;

        /// <summary>
        /// The coded lookups
        /// </summary>
        private Dictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> _codedLookups;

        /// <summary>
        /// The time restricted lookups
        /// </summary>
        private Dictionary<TypeOfLimitedLifeLookup, IDictionary<string, ValidityPeriods>> _limitedLifeLookups;

        /// <summary>
        /// The item lookups
        /// </summary>
        private Dictionary<TypeOfListItemLookup, IDictionary<string, IReadOnlyCollection<string>>> _listItemLookups;

        public IAcademicYear AcademicYear { get; set; }

        /// <summary>
        /// Gets the simple lookups.
        /// </summary>
        public IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> SimpleLookups
        {
            get
            {
                return _simpleLookups
                  ?? (_simpleLookups = new Dictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>>());
            }
        }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        public IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> CodedLookups
        {
            get
            {
                return _codedLookups
                  ?? (_codedLookups = new Dictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>>());
            }
        }

        public IDictionary<TypeOfListItemLookup, IDictionary<string, IReadOnlyCollection<string>>> ListItemLookups
        {
            get
            {
                return _listItemLookups
                       ?? (_listItemLookups = new Dictionary<TypeOfListItemLookup, IDictionary<string, IReadOnlyCollection<string>>>());
            }
        }

        /// <summary>
        /// Gets the time limited lookups.
        /// </summary>
        public IDictionary<TypeOfLimitedLifeLookup, IDictionary<string, ValidityPeriods>> LimitedLifeLookups
        {
            get
            {
                return _limitedLifeLookups
                  ?? (_limitedLifeLookups = new Dictionary<TypeOfLimitedLifeLookup, IDictionary<string, ValidityPeriods>>());
            }
        }
    }
}
