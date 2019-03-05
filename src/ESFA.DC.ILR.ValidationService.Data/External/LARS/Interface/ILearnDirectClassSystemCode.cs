using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// i learn direct class system code definition
    /// </summary>
    public interface ILearnDirectClassSystemCode
    {
        /// <summary>
        /// Gets the code.
        /// </summary>
        string Code { get; }
    }

    /// <summary>
    /// the learn direct class system code helper
    /// </summary>
    public static class LearnDirectClassSystemCodeHelper
    {
        /// <summary>
        /// Determines whether this instance is known.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if this instance is known; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKnown(this ILearnDirectClassSystemCode source) =>
            It.Has(source)
                && It.Has(source.Code)
                && !source.Code.ComparesWith("NUL");
    }
}
