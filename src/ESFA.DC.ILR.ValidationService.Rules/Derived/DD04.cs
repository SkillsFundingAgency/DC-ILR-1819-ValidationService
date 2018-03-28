using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DD04 : IDD04
    {
        public DateTime? Derive(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery learningDelivery)
        {
            return EarliestLearningDeliveryLearnStartDateFor(learningDeliveries, 1, learningDelivery.ProgTypeNullable, learningDelivery.FworkCodeNullable, learningDelivery.PwayCodeNullable);
        }

        public DateTime? EarliestLearningDeliveryLearnStartDateFor(IEnumerable<ILearningDelivery> learningDeliveries, long aimType, long? progType, long? fworkCode, long? pwayCode)
        {
            return learningDeliveries
                    .Where(ld =>
                    ld.AimType == aimType
                    && ld.ProgTypeNullable == progType
                    && ld.FworkCodeNullable == fworkCode
                    && ld.PwayCodeNullable == pwayCode)
                .Min(ld => (DateTime?)ld.LearnStartDate);
        }
    }
}
