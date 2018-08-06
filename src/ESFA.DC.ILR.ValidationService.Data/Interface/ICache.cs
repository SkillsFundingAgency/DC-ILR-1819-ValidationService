namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface ICache<out T>
    {
        T Item { get; }
    }
}
