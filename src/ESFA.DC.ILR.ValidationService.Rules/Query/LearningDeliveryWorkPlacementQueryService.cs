using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryWorkPlacementQueryService : ILearningDeliveryWorkPlacementQueryService
    {
        public bool HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements, DateTime? learnActEndDate)
        {
            return learningDeliveryWorkPlacements != null
                   && learnActEndDate != null
                   && learningDeliveryWorkPlacements.Any(ldwp => ldwp.WorkPlaceEndDateNullable > learnActEndDate);
        }

        public bool HasAnyEmpIdNullAndStartDateNotNull(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements)
        {
            return learningDeliveryWorkPlacements != null
                   && learningDeliveryWorkPlacements
                       .Any(ldwp => ldwp.WorkPlaceEmpIdNullable == null && ldwp.WorkPlaceStartDate != null);
        }
    }
}
