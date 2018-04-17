﻿using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ExternalDataCacheStub : IExternalDataCache
    {
        public ExternalDataCacheStub()
        {
            ULNs = new List<long>();
            LearningDeliveries = new Dictionary<string, LearningDelivery>();
            UKPRNs = new List<long>();
        }

        public IReadOnlyCollection<long> ULNs { get; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        public IReadOnlyCollection<long> UKPRNs { get; }
    }
}