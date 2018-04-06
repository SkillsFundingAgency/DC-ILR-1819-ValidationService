using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.FileData.Interface
{
    public interface IFileDataCachePopulationService
    {
        void Populate(IMessage message);
    }
}
