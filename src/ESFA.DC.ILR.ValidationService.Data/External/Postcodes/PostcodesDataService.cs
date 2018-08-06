using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    public class PostcodesDataService : IPostcodesDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        public PostcodesDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public bool PostcodeExists(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode)
                   && _externalDataCache.Postcodes.Contains(postcode);
        }
    }
}
