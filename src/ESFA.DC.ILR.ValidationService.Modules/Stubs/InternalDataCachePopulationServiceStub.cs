using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ESFA.DC.ILR.ValidationService.InternalData;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
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

            internalDataCache.AimTypes = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "AimType"));
            internalDataCache.CompStatuses = new HashSet<int>(BuildSimpleLookupEnumerable<int>(lookups, "CompStatus"));
        }

        public IEnumerable<T> BuildSimpleLookupEnumerable<T>(XElement lookups, string type)
        {
            return lookups
                .Descendants(type)
                .Descendants("option")
                .Attributes("code")
                .Select(c => (T)Convert.ChangeType(c.Value, typeof(T)));
        }
    }
}
