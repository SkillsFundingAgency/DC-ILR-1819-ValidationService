namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPopulationService
    {
        void Populate();
    }

    public interface IPopulationService<T>
        where T : class
    {
        void Populate(T data);
    }
}
