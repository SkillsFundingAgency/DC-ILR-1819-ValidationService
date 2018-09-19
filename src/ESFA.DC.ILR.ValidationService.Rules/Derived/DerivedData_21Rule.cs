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
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// IsAdultFundedUnemployedWithOtherStateBenefits
    /// </summary>
    public class DerivedData_21Rule :
        IDerivedData_21Rule
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
        /// Determines whether [is not employed] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployed(ILearnerEmploymentStatus candidate)
        {
            return It.IsInRange(
                    candidate?.EmpStat,
                    TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                    TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);
        }

        /// <summary>
        /// In receipt of another benefit.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfAnotherBenefit(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                EmploymentStatusMonitoring.InReceiptOfAnotherStateBenefit);

        /// <summary>
        /// In receipt of universal credit.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfUniversalCredit(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                EmploymentStatusMonitoring.InReceiptOfUniversalCredit);

        /// <summary>
        /// Determines whether [is not employed] [the specified learner employment status].
        /// </summary>
        /// <param name="learnerEmploymentStatus">The learner employment status.</param>
        /// <param name="delivery">The delivery.</param>
        /// <param name="monitoredAndNotMandated">The monitored and not mandated.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed] [the specified learner employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployed(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatus, ILearningDelivery delivery, Func<bool> monitoredAndNotMandated)
        {
            It.IsNull(monitoredAndNotMandated)
                .AsGuard<ArgumentNullException>(nameof(monitoredAndNotMandated));

            var candidate = learnerEmploymentStatus
                    .Where(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                    .OrderByDescending(x => x.DateEmpStatApp)
                    .FirstOrDefault();

            var esms = candidate?.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return IsNotEmployed(candidate) && (esms.SafeAny(InReceiptOfAnotherBenefit) || (esms.SafeAny(InReceiptOfUniversalCredit) && monitoredAndNotMandated()));
        }

        /// <summary>
        /// Determines whether the specified FAM is (learning dleivery) monitored.
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>
        ///   <c>true</c> if the specified fam is monitored; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMonitored(ILearningDeliveryFAM fam) =>
            It.IsInRange(fam.LearnDelFAMType, DeliveryMonitoring.Types.Learning);

        /// <summary>
        /// Determines whether (any of) the specified fams are monitored.
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>
        ///   <c>true</c> if the specified fams are (learning delivery) monitored; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMonitored(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.Any(IsMonitored);

        /// <summary>
        /// Mandateds to skills training.
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>true if the condition is met</returns>
        public bool MandatedToSkillsTraining(ILearningDeliveryFAM fam) =>
            It.IsInRange($"{fam.LearnDelFAMType}{fam.LearnDelFAMCode}", DeliveryMonitoring.MandationToSkillsTraining);

        /// <summary>
        /// Mandated to skills training.
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>true if the condition is met</returns>
        public bool MandatedToSkillsTraining(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.Any(MandatedToSkillsTraining);

        /// <summary>
        /// Confirms the monitoring and mandation.
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>true if the condition is met</returns>
        public bool ConfirmMonitoringAndMandation(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            IsMonitored(fams) && !MandatedToSkillsTraining(fams);

        /// <summary>
        /// Determines whether [is adult funded unemployed with other state benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded unemployed with other state benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearner candidate)
        {
            It.IsNull(candidate)
                .AsGuard<ArgumentNullException>(nameof(candidate));

            var lds = candidate.LearningDeliveries.AsSafeReadOnlyList();
            var les = candidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35

                    //  is umemployed (not employed, seeking and available or otherwise)
                    and     LearnerEmploymentStatus.EmpStat = 11 or 12 for the latest Employment Status on (or before) the LearningDelivery.LearnStartDate

                            // in receipt of another benefit.
                    and     ((EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 3)
                            or
                            // in receipt of universal credit.
                            (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 4
                            // is learning delivery monitored
                            and LearningDeliveryFAM.LearnDelFAMType = LDM
                            // and not mandated to skills training
                            and LearningDeliveryFAM.LearnDelFAMCode <> 318))

                        set to Y,
                        otherwise set to N
             */

            return lds.Any(d =>
                IsAdultSkills(d)
                && IsNotEmployed(les, d, () => ConfirmMonitoringAndMandation(d.LearningDeliveryFAMs.AsSafeReadOnlyList())));
        }
    }
}
