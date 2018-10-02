using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public sealed class ErrorLookupPopulationService : IErrorLookupPopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly IValidationErrorsDataRetrievalService _validationErrorsDataRetrievalService;

        public ErrorLookupPopulationService(
            IExternalDataCache externalDataCache,
            IValidationErrorsDataRetrievalService validationErrorsDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _validationErrorsDataRetrievalService = validationErrorsDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.ValidationErrors = await _validationErrorsDataRetrievalService.RetrieveAsync(cancellationToken);
        }
    }
}
