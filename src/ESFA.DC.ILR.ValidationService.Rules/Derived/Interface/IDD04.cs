using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDD04
    {
        DateTime? Derive(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery learningDelivery);
    }
}
