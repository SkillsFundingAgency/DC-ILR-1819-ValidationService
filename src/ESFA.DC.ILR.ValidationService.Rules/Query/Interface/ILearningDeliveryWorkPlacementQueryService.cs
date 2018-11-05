using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryWorkPlacementQueryService
    {
        bool HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements, DateTime? learnActEndDate);

        bool HasAnyEmpIdNullAndStartDateNotNull(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements);
    }
}
