using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.TTAccom
{
    /// <summary>
    /// i provide term time accomodation details
    /// </summary>
    public interface IProvideTermTimeAccomodationDetails
    {
        /// <summary>
        /// Determines whether [contains] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(int candidate);

        /// <summary>
        /// Determines whether the specified candidate is current.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is current; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(int candidate, DateTime reference);
    }
}
