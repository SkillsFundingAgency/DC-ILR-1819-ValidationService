using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IMessageCachePopulationService : IPopulationService
    {
        void Populate(IMessage data);
    }
}
