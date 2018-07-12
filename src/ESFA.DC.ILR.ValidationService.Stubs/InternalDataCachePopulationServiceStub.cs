using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class InternalDataCachePopulationServiceStub : IInternalDataCachePopulationService
    {
        private readonly IInternalDataCache _internalDataCache;

        public InternalDataCachePopulationServiceStub(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public void Populate()
        {
            var internalDataCache = (InternalDataCache)_internalDataCache;

            XElement lookups;

            using (var stream = new FileStream(@"Files/Lookups.xml", FileMode.Open))
            {
                lookups = XElement.Load(stream);
            }

            internalDataCache.AcademicYear = BuildAcademicYear();

            internalDataCache.AimTypes = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "AimType"));
            internalDataCache.CompStatuses = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "CompStatus"));
            internalDataCache.EmpOutcomes = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "EmpOutcome"));
            internalDataCache.FundModels = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "FundModel"));
            internalDataCache.QUALENT3s = new HashSet<string>(BuildSimpleLookupEnumerable<string>(lookups, "QualEnt3"));
        }

        private AcademicYear BuildAcademicYear()
        {
            return new AcademicYear()
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                Start = new DateTime(2018, 8, 1),
            };
        }

        private IEnumerable<T> BuildSimpleLookupEnumerable<T>(XElement lookups, string type)
        {
            return lookups
                .Descendants(type)
                .Descendants("option")
                .Attributes("code")
                .Select(c => (T)Convert.ChangeType(c.Value, typeof(T)));
        }
    }
}
