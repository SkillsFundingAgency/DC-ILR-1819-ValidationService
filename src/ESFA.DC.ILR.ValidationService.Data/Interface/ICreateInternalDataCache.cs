namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// i create the internal data cache
    /// this contract is here because the reference order is wrong
    /// the factory class contracts are not available for injection to the cache classes.
    /// </summary>
    public interface ICreateInternalDataCache
    {
        /// <summary>
        /// Creates an instance of the internal data cache.
        /// </summary>
        /// <returns>an internal data cache</returns>
        IInternalDataCache Create();
    }
}
