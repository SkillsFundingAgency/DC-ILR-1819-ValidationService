using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Interface.Utility
{
    /// <summary>
    /// caters for the execution of safe operations
    /// </summary>
    public static class SafeActions
    {
        /// <summary>
        /// Tries the specified action.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>the result of the operation or the result of a handler should the operation fail</returns>
        public static TResult Try<TResult>(Func<TResult> action, Func<TResult> handler = null)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                return It.Has(handler)
                    ? handler.Invoke()
                    : default(TResult);
            }
        }
    }
}
