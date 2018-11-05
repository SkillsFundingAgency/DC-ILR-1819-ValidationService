using System;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3
{
    public class QUALENT3DataService : IQUALENT3DataService
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public QUALENT3DataService(IProvideLookupDetails provideLookupDetails)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public bool Exists(string qualent3)
        {
            return _provideLookupDetails.Contains(LookupTimeRestrictedKey.QualEnt3, qualent3);
        }

        public bool IsLearnStartDateBeforeValidTo(string qualent3, DateTime learnStartDate)
        {
            return _provideLookupDetails.IsCurrent(LookupTimeRestrictedKey.QualEnt3, qualent3, learnStartDate);
        }
    }
}
