using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars framework type definition
    /// </summary>
    public interface ILARSFramework
    {
        /// <summary>
        /// Gets the framework code.
        /// </summary>
        int FworkCode { get; }

        /// <summary>
        /// Gets the programme type.
        /// </summary>
        int ProgType { get; }

        /// <summary>
        /// Gets the pathway code.
        /// </summary>
        int PwayCode { get; }

        /// <summary>
        /// Gets the framework aims.
        /// </summary>
        IEnumerable<ILARSFrameworkAim> FrameworkAims { get; }

        /// <summary>
        /// Gets the framework common components.
        /// </summary>
        IEnumerable<ILARSFrameworkCommonComponent> FrameworkCommonComponents { get; }
    }
}
