using System;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class PostcodesDataServiceStub : IPostcodesDataService
    {
        public bool PostcodeExists(string postcode)
        {
            return true;
        }
    }
}
