using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using System.Collections.Generic;
using IAcademicYear = ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface.IAcademicYear;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IInternalDataCache
    {
        IAcademicYear AcademicYear { get; }

        /// <summary>
        /// Gets the simple lookups.
        /// </summary>
        IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> SimpleLookups { get; }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> CodedLookups { get; }

        /// <summary>
        /// Gets the time restricted lookups.
        /// </summary>
        IDictionary<TypeOfLimitedLifeLookup, IDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; }

        /// <summary>
        /// Gets the list item lookups.
        /// </summary>
        IDictionary<TypeOfListItemLookup, IDictionary<string, IReadOnlyCollection<string>>> ListItemLookups { get; }
    }
}
