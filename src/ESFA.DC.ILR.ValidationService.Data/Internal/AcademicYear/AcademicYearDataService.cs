using System;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear
{
    public class AcademicYearDataService : IAcademicYearDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public AcademicYearDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public DateTime AugustThirtyFirst()
        {
            return _internalDataCache.AcademicYear.AugustThirtyFirst;
        }

        public DateTime End()
        {
            return _internalDataCache.AcademicYear.End;
        }

        public DateTime JanuaryFirst()
        {
            return _internalDataCache.AcademicYear.JanuaryFirst;
        }

        public DateTime Start()
        {
            return _internalDataCache.AcademicYear.Start;
        }
    }
}
