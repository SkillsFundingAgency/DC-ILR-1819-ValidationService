﻿using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// i support funding withdrawal
    /// for the use-case where LARS 'effective to' dates are set
    /// one day before the 'effective from' dates as a mechanism to remove funding. 'start
    ///  date' and 'end date' have been used over 'effective from' and 'effective to'
    /// for clarity and consistency
    /// </summary>
    public interface ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        DateTime? EndDate { get; }
    }

    /// <summary>
    /// the funding withdrawal helper grafts the necessary routines onto any supporting entities
    /// these rouintes will be avaiable during testing ensuring consistency and a lower impact change
    /// </summary>
    public static class FundingWithdrawalHelper
    {
        /// <summary>
        /// Determines whether this instance is withdrawn.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the specified source is withdrawn; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWithdrawn(this ISupportFundingWithdrawal source) =>
            It.Has(source)
            && source.EndDate < source.StartDate;

        /// <summary>
        /// Determines whether the specified candidate is current.
        /// this caters for and evaluates if funding has been withdrawn
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="optionalEnding">The optional ending.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCurrent(this ISupportFundingWithdrawal source, DateTime candidate, DateTime? optionalEnding = null) =>
            It.Has(source)
            && !source.IsWithdrawn()
            && It.IsBetween(candidate, source.StartDate, optionalEnding ?? source.EndDate ?? DateTime.MaxValue);
    }
}
