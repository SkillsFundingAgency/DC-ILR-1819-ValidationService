using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_06Rule : IDerivedData_06Rule
    {
        public DateTime Derive(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Min(ld => ld.LearnStartDate);
        }
    }
}
