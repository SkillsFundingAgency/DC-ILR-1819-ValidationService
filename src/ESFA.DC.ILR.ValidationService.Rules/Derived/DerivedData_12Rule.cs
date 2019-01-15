using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_12Rule : IDerivedData_12Rule //: IDerivedData_32Rule
    {
        private readonly HashSet<int> ValidEmploymentStatusCodes = new HashSet<int>() { 1, 2 };
        private const int ValidEmploymentStatusCodeForLDM = 4;

        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public DerivedData_12Rule(
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
        {
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public bool IsAdultSkillsFundedOnBenefits(
            IReadOnlyCollection<ILearnerEmploymentStatus> employmentStatusMonitorings,
            ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                   && EmploymentStatusMonitoringConditionMet(employmentStatusMonitorings, learningDelivery);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.AdultSkills;
        }

        public bool EmploymentStatusMonitoringConditionMet(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses, ILearningDelivery learningDelivery)
        {
            var latest = learnerEmploymentStatuses
                .Where(x => x.DateEmpStatApp <= learningDelivery.LearnStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

            if (_learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    latest,
                    Monitoring.EmploymentStatus.Types.BenefitStatusIndicator,
                    ValidEmploymentStatusCodes))
            {
                return true;
            }

            if (FamConditionMet(learningDelivery.LearningDeliveryFAMs) &&
                 _learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(
                    latest,
                    Monitoring.EmploymentStatus.Types.BenefitStatusIndicator,
                    ValidEmploymentStatusCodeForLDM))
            {
                return true;
            }

            return false;
        }

        public bool FamConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFams,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_MandationtoSkillsTraining);
        }
    }
}
