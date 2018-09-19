using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryQueryService
    {
        double? AverageAddHoursPerLearningDay(ILearningDelivery learningDelivery);

        int LearningDaysForLearningDelivery(ILearningDelivery learningDelivery);
    }
}
