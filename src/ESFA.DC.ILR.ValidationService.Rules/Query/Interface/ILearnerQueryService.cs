using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerQueryService
    {
        bool HasLearningDeliveryFAMCodeForType(ILearner learner, string famType, string famCode);
    }
}
