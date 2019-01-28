﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    /// <summary>
    /// learn aim ref rule 89
    /// </summary>
    /// <seealso cref="LearnAimRefRuleBase" />
    public class LearnAimRef_89Rule :
        LearnAimRefRuleBase
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = "LearnAimRef_89";

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_89Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data 07 (rule).</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        /// <param name="yearService">The year service.</param>
        public LearnAimRef_89Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_11Rule derivedData11,
            IAcademicYearDataService yearService)
                : base(validationErrorHandler, larsData, derivedData07, derivedData11)
        {
            It.IsNull(yearService)
                .AsGuard<ArgumentNullException>(nameof(yearService));

            YearData = yearService;
        }

        /// <summary>
        /// Gets the (academic) year data.
        /// </summary>
        /// <value>
        /// The year data.
        /// </value>
        public IAcademicYearDataService YearData { get; }

        /// <summary>
        /// Gets the closing date of last academic year.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the closing date of the last academic year</returns>
        public DateTime GetClosingDateOfLastAcademicYear(ILearningDelivery delivery) =>
            YearData.GetAcademicYearOfLearningDate(delivery.LearnStartDate, AcademicYearDates.PreviousYearEnd);

        /// <summary>
        /// Determines whether [has valid learning aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchCategory">The branch category.</param>
        /// <returns>
        ///   <c>true</c> if [has valid learning aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidLearningAim(ILearningDelivery delivery, string branchCategory)
        {
            var lastYearEnd = GetClosingDateOfLastAcademicYear(delivery);
            var validity = LarsData.GetValiditiesFor(delivery.LearnAimRef)
                .Where(x => x.ValidityCategory.ComparesWith(branchCategory))
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

            return (validity?.EndDate ?? DateTime.MaxValue) > lastYearEnd;
        }

        /// <summary>
        /// Gets the (rule) name.
        /// </summary>
        /// <returns>
        /// the rule name
        /// </returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branch">The branch result.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public override bool PassesConditions(ILearningDelivery delivery, BranchResult branch)
        {
            /*
            Where the learning aim validity criteria has been met in Table 1,
            the record with the latest validity start date in the LARS database for this Learning aim reference
            must have a validity end date (if entered) on or after the beginning of the current teaching year

            And in ’structured’ English:

            for the latest LARS_Validity (max LARS_Validity_StartDate) matching the LearningDelivery_LearnAimRef
            if      LARS_Validity_EndDate != null
            and     LARS_Validity_EndDate >= CommencementDateOfCurrentAcademicYear (i.e. 2018-08-01)
                        return OK
            otherwise   return error
            */

            return branch.OutOfScope || HasValidLearningAim(delivery, branch.Category);
        }
    }
}
