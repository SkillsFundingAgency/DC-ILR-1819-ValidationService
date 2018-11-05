using Moq.Language.Flow;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules
{
    /// <summary>
    /// some static helpful mocking routines
    /// </summary>
    public static class MoqUtilities
    {
        /// <summary>
        /// Returns in order; supports sequential operations on Moqs
        /// </summary>
        /// <typeparam name="T">the incoming type</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="setup">The setup.</param>
        /// <param name="results">The results.</param>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results)
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }

        /// <summary>
        /// Returns in order; supports sequential operations on Moqs
        /// </summary>
        /// <typeparam name="T">the incoming type</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="setup">The setup.</param>
        /// <param name="results">The results.</param>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, IEnumerable<TResult> results)
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}
