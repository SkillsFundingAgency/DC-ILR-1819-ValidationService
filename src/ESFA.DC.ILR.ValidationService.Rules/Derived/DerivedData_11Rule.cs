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
    /// derived data rule 11
    /// Adult skills funded learner on benefits on learning aim start date
    /// </summary>
    public class DerivedData_11Rule :
        IDerivedData_11Rule
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
        /// In receipt of benefits.
        /// </summary>
        /// <param name="learnerEmploymentStatus">The learner employment status.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>true, if on state benefits at start of learning aim</returns>
        public bool InReceiptOfBenefits(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatus, ILearningDelivery delivery)
        {
            var candidate = learnerEmploymentStatus
                    .Where(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                    .OrderByDescending(x => x.DateEmpStatApp)
                    .FirstOrDefault();

            var esms = candidate?.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return esms.SafeAny(InReceiptOfBenefits);
        }

        public bool InReceiptOfBenefits(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfUniversalCredit,
                Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit,
                Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance,
                Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance);

        public bool IsAdultFundedOnBenefitsAtStartOfAim(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmployments)
        {
            It.IsNull(delivery)
                .AsGuard<ArgumentNullException>(nameof(delivery));
            It.IsEmpty(learnerEmployments)
                .AsGuard<ArgumentNullException>(nameof(learnerEmployments));

            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35
                    and the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim
                    is (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 1, 2, 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            return IsAdultSkills(delivery)
                && InReceiptOfBenefits(learnerEmployments, delivery);
        }
    }
}
