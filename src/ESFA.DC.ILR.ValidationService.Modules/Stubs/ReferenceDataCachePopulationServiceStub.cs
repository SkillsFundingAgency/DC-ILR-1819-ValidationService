using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class ReferenceDataCachePopulationServiceStub : IExternalDataCachePopulationService<ILearner>
    {
        public void Populate(IEnumerable<ILearner> validationItems)
        {
        }
    }
}
