﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class ReferenceDataCachePopulationServiceStub : IReferenceDataCachePopulationService<ILearner>
    {
        public void Populate(IReferenceDataCache referenceDataCache, IEnumerable<ILearner> validationItems)
        {
        }
    }
}
