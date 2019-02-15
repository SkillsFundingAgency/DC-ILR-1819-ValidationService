using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal
{
    public class InternalDataCache : IInternalDataCache
    {
        /// <summary>
        /// The simple lookups
        /// </summary>
        private Dictionary<TypeOfIntegerCodedLookup, IContainThis<int>> _integerLookups;

        /// <summary>
        /// The coded lookups
        /// </summary>
        private Dictionary<TypeOfStringCodedLookup, IContainThis<string>> _codedLookups;

        /// <summary>
        /// The time restricted lookups
        /// </summary>
        private Dictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> _limitedLifeLookups;

        /// <summary>
        /// The item lookups
        /// </summary>
        private Dictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IContainThis<string>>> _listItemLookups;

        /// <summary>
        /// Gets or sets the academic year.
        /// </summary>
        public IAcademicYear AcademicYear { get; set; }

        /// <summary>
        /// Gets the integer lookups.
        /// </summary>
        public IDictionary<TypeOfIntegerCodedLookup, IContainThis<int>> IntegerLookups
        {
            get
            {
                return _integerLookups
                  ?? (_integerLookups = new Dictionary<TypeOfIntegerCodedLookup, IContainThis<int>>());
            }
        }

        /// <summary>
        /// Gets the string lookups.
        /// </summary>
        public IDictionary<TypeOfStringCodedLookup, IContainThis<string>> StringLookups
        {
            get
            {
                return _codedLookups
                  ?? (_codedLookups = new Dictionary<TypeOfStringCodedLookup, IContainThis<string>>());
            }
        }

        public IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IContainThis<string>>> ListItemLookups
        {
            get
            {
                return _listItemLookups
                       ?? (_listItemLookups = new Dictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IContainThis<string>>>());
            }
        }

        /// <summary>
        /// Gets the time limited lookups.
        /// </summary>
        public IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> LimitedLifeLookups
        {
            get
            {
                return _limitedLifeLookups
                  ?? (_limitedLifeLookups = new Dictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>>());
            }
        }
    }
}
