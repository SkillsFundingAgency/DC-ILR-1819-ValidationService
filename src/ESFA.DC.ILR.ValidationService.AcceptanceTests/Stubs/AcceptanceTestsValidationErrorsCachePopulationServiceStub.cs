using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests.Stubs
{
    public sealed class AcceptanceTestsValidationErrorsCachePopulationServiceStub : IErrorLookupPopulationService
    {
        private readonly ExternalDataCache _dataCache;

        public AcceptanceTestsValidationErrorsCachePopulationServiceStub(IExternalDataCache iCache)
        {
            _dataCache = (ExternalDataCache)iCache;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            _dataCache.ValidationErrors = new Dictionary<string, ValidationError>();
        }
    }
}
