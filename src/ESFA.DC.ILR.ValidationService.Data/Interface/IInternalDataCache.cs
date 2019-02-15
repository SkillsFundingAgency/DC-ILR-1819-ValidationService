using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Utility;
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
        IDictionary<TypeOfIntegerCodedLookup, IContainThis<int>> IntegerLookups { get; }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        IDictionary<TypeOfStringCodedLookup, IContainThis<string>> StringLookups { get; }

        /// <summary>
        /// Gets the time restricted lookups.
        /// </summary>
        IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; }

        /// <summary>
        /// Gets the list item lookups.
        /// </summary>
        IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IContainThis<string>>> ListItemLookups { get; }
    }
}
