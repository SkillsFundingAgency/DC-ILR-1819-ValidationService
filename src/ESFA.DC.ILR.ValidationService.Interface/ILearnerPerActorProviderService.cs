﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ILearnerPerActorProviderService
    {
        Task<IEnumerable<IMessage>> ProvideAsync();
    }
}