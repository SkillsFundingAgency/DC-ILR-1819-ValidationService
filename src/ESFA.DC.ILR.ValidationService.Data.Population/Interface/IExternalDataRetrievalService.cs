namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataRetrievalService<out TOut>
    {
        TOut Retrieve();
    }
}
