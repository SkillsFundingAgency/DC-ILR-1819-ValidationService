using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPreValidationPopulationService : IPopulationService
    {
    }

    public interface IPreValidationPopulationService<T> : IPopulationService<T>
        where T : class
    {
    }
}
