using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_17Rule
    {
        bool IsTotalNegotiatedPriceMoreThanCapForStandardLearningDeliveries(
            IReadOnlyCollection<ILearningDelivery> standardLearningDeliveries);

        bool IsTotalNegotiatedPriceMoreThanCapForStandard(
            IReadOnlyCollection<ILearningDelivery> learningDeliveries,
            int? standardCode);
    }
}
