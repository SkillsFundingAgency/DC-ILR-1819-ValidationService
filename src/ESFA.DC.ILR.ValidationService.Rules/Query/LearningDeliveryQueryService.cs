using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryQueryService : ILearningDeliveryQueryService
    {
        public virtual double? AverageAddHoursPerLearningDay(ILearningDelivery learningDelivery)
        {
            if (!learningDelivery.AddHoursNullable.HasValue)
            {
                return null;
            }

            var days = LearningDaysForLearningDelivery(learningDelivery);

            return (double)learningDelivery.AddHoursNullable.Value / days;
        }

        public virtual int LearningDaysForLearningDelivery(ILearningDelivery learningDelivery)
        {
            return (learningDelivery.LearnPlanEndDate - learningDelivery.LearnStartDate).Days + 1;
        }
    }
}
