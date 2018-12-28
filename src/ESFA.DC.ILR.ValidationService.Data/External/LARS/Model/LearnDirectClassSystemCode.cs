using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the learn direct class system code implementation
    /// </summary>
    /// <seealso cref="ILearnDirectClassSystemCode" />
    public class LearnDirectClassSystemCode :
        ILearnDirectClassSystemCode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDirectClassSystemCode"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public LearnDirectClassSystemCode(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        public string Code { get; }
    }
}
