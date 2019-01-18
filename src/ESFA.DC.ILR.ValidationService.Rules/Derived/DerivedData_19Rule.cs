using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_19Rule : IDerivedData_19Rule
    {
        public DateTime? Derive(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery learningDelivery)
        {
            var programmeAims = LearningDeliveriesForProgrammeAim(learningDeliveries);

            if (LearningDeliveryHasApprenticeshipStandardType(learningDelivery) && programmeAims.Any())
            {
                return LatestLearningDeliveryLearnPlanEndDateFor(programmeAims, learningDelivery.ProgTypeNullable, learningDelivery.StdCodeNullable);
            }

            return null;
        }

        public DateTime? LatestLearningDeliveryLearnPlanEndDateFor(IEnumerable<ILearningDelivery> learningDeliveries, long? progType, long? stdCode)
        {
            return learningDeliveries
                    .Where(ld =>
                    ld.ProgTypeNullable == progType
                    && ld.StdCodeNullable == stdCode)
                .Max(ld => (DateTime?)ld.LearnPlanEndDate);
        }

        public bool LearningDeliveryHasApprenticeshipStandardType(ILearningDelivery learningDelivery)
        {
            return learningDelivery != null
                ? learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                : false;
        }

        public IEnumerable<ILearningDelivery> LearningDeliveriesForProgrammeAim(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries?
                .Where(ld =>
                ld.AimType == TypeOfAim.ProgrammeAim).ToList() ?? new List<ILearningDelivery>();
        }
    }
}
