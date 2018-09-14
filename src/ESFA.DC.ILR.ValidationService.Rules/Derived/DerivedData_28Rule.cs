using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 28
    /// Adult skills funded learner unemployed with benefits on learning aim start date
    /// IsAdultFundedUnemployedWithBenefits
    /// </summary>
    public class DerivedData_28Rule :
        IDerivedData_28Rule
    {
        /// <summary>
        /// Determines whether [is adult skills] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is adult skills] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultSkills(ILearningDelivery delivery) =>
                    delivery.FundModel == TypeOfFunding.AdultSkills;

        /// <summary>
        /// In receipt of employment support.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfEmploymentSupport(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                EmploymentStatusMonitoring.InReceiptOfJobSeekersAllowance,
                EmploymentStatusMonitoring.InReceiptOfEmploymentAndSupportAllowance);

        /// <summary>
        /// Determines whether [has valid employment status] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has valid employment status] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidEmploymentStatus(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(
                candidate?.EmpStat,
                TypeOfEmploymentStatus.InPaidEmployment,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotKnownProvided);

        /// <summary>
        /// Determines whether [is valid with employment support] [the specified candidates].
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid with employment support] [the specified candidates]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidWithEmploymentSupport(IReadOnlyCollection<ILearnerEmploymentStatus> candidates, ILearningDelivery delivery)
        {
            var candidate = candidates
                    .Where(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                    .OrderByDescending(x => x.DateEmpStatApp)
                    .FirstOrDefault();

            var esms = candidate?.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return HasValidEmploymentStatus(candidate) && esms.SafeAny(InReceiptOfEmploymentSupport);
        }

        /// <summary>
        /// Determines whether [is valid with employment support] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is valid with employment support] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidWithEmploymentSupport(ILearner candidate)
        {
            var lds = candidate.LearningDeliveries.AsSafeReadOnlyList();
            var les = candidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

            return lds.Any(d => IsAdultSkills(d) && IsValidWithEmploymentSupport(les, d));
        }

        /// <summary>
        /// In receipt of (other state) benefits / credits.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfCredits(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                EmploymentStatusMonitoring.InReceiptOfAnotherStateBenefit,
                EmploymentStatusMonitoring.InReceiptOfUniversalCredit);

        /// <summary>
        /// Determines whether [is not employed] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployed(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(
                candidate.EmpStat,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);

        /// <summary>
        /// Determines whether [is not employed with benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployedWithBenefits(ILearnerEmploymentStatus candidate)
        {
            var esms = candidate.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return IsNotEmployed(candidate) && esms.Any(InReceiptOfCredits);
        }

        /// <summary>
        /// Determines whether [is not employed with benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployedWithBenefits(ILearner candidate) =>
            candidate.LearnerEmploymentStatuses.SafeAny(IsNotEmployedWithBenefits);

        /// <summary>
        /// Determines whether [is working short hours] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is working short hours] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorkingShortHours(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                EmploymentStatusMonitoring.EmployedForLessThan16HoursPW,
                EmploymentStatusMonitoring.EmployedFor0To10HourPW,
                EmploymentStatusMonitoring.EmployedFor11To20HoursPW);

        /// <summary>
        /// Determines whether the specified candidate is employed.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is employed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmployed(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(candidate.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Determines whether [is employed with support] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is employed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmployedWithsupport(ILearnerEmploymentStatus candidate)
        {
            var esms = candidate.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return IsEmployed(candidate) && esms.Any(IsWorkingShortHours) && esms.Any(InReceiptOfCredits);
        }

        /// <summary>
        /// Determines whether [is employed with support] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is employed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmployedWithSupport(ILearner candidate) =>
            candidate.LearnerEmploymentStatuses.SafeAny(IsEmployedWithsupport);

        /// <summary>
        /// Determines whether [is adult skills unemployed with benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult skills unemployed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithBenefits(ILearner candidate)
        {
            It.IsNull(candidate)
                .AsGuard<ArgumentNullException>(nameof(candidate));

            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35
                    // and has valid employment status
                    and LearnerEmploymentStatus.EmpStat = 10, 11, 12 or 98
                    // and in receipt of support at the time of starting the learning aim
                    and (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 1 or 2)
                        (for the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim)
                or
                    // or is not employed, and in receipt of benefits
                    LearnerEmploymentStatus.EmpStat = 11 or 12
                    and (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 3 or 4)
                or
                    // or is employed with workng short hours and in receipt of support
                    LearnerEmploymentStatus.EmpStat = 10
                    and (EmploymentStatusMonitoring.ESMType = EII and EmploymentStatusMonitoring.ESMCode = 2, 5 or 6)
                    and (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            return IsValidWithEmploymentSupport(candidate)
                || IsNotEmployedWithBenefits(candidate)
                || IsEmployedWithSupport(candidate);
        }
    }
}
