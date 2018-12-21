using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using System.Collections.Generic;
using IAcademicYear = ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface.IAcademicYear;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IInternalDataCache
    {
        IAcademicYear AcademicYear { get; }

        IReadOnlyCollection<int> AimTypes { get; }

        IReadOnlyCollection<int> CompStatuses { get; }

        IReadOnlyCollection<int> EmpOutcomes { get; }

        IReadOnlyCollection<int> FundModels { get; }

        IDictionary<int, ValidityPeriods> LLDDCats { get; }

        IDictionary<string, ValidityPeriods> QUALENT3s { get; }

        /// <summary>
        /// Gets the simple lookups.
        /// </summary>
        IDictionary<LookupSimpleKey, IReadOnlyCollection<int>> SimpleLookups { get; }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        IDictionary<LookupCodedKey, IReadOnlyCollection<string>> CodedLookups { get; }

        /// <summary>
        /// Gets the coded lookups
        /// </summary>
        IDictionary<LookupCodedKeyDictionary, IDictionary<string, IReadOnlyCollection<string>>> CodedDictionaryLookups { get; }

        /// <summary>
        /// Gets the time restricted lookups.
        /// </summary>
        IDictionary<LookupTimeRestrictedKey, IDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; }

        IDictionary<LookupComplexKey, IDictionary<string, IDictionary<string, ValidityPeriods>>> CodedComplexLookups
        {
            get;
        }
    }
}
