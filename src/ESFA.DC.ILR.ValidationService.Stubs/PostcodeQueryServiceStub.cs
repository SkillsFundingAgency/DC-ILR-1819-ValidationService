using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class PostcodeQueryServiceStub : IPostcodeQueryService
    {
        public bool RegexValid(string postcode)
        {
            return true;
        }
    }
}
