using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    using System;
    using System.Linq;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
    using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
    using ESFA.DC.ILR.ValidationService.Utility;

    public class LearnDelFAMType_60Rule : AbstractRule, IRule<ILearner>
    {
        public const string MessagePropertyName = "LearnDelFAMType";
        public const string Name = "LearnDelFAMType_60";
        private const string _legalOrgType = "USDC";
        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILARSDataService _larsData;
        private readonly IDD07 _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;

        public LearnDelFAMType_60Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDD07 derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_60)
        {
            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
        }

        public DateTime LastInviableStartDate => new DateTime(2016, 07, 31);

        public DateTime LastInviableEndDate => new DateTime(2017, 08, 01);

        public TimeSpan MinimumViableAge => new TimeSpan(8766, 0, 0, 0); // 24 years

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition)
        {
            return candidate.LearningDeliveries.SafeAny(matchCondition);
        }

        public bool IsApprenticeship(ILearningDelivery delivery)
        {
            return _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);
        }

        public bool IsInflexibleElementOfTrainingAim(ILearner candidate)
        {
            return _derivedData29.IsInflexibleElementOfTrainingAim(candidate);
        }

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition)
        {
            return delivery.LearningDeliveryFAMs.SafeAny(matchCondition);
        }

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);
        }

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);
        }

        public bool IsAdultFundedUnemployedWithBenefits(ILearner candidate)
        {
            return _derivedData28.IsAdultFundedUnemployedWithBenefits(candidate);
        }

        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearner candidate)
        {
            return _derivedData21.IsAdultFundedUnemployedWithOtherStateBenefits(candidate);
        }

        public bool IsRestart(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);
        }

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var deliveries = _larsData.GetDeliveriesFor(delivery.LearnAimRef)
                .Where(x => It.IsBetween(delivery.LearnStartDate, x.EffectiveFrom, x.EffectiveTo ?? DateTime.MaxValue))
                .AsSafeReadOnlyList();

            return deliveries
                .SelectMany(x => x.AnnualValues.AsSafeReadOnlyList())
                .Any(x => IsBasicSkillsLearner(x));
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor)
        {
            return It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);
        }

        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining);
        }

        public bool IsLegalOrgTypeMatchForUkprn()
        {
            var ukprn = _fileDataService.UKPRN();
            return _organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, _legalOrgType);
        }

        public bool IsViableStart(ILearningDelivery delivery)
        {
            return delivery.LearnStartDate > LastInviableStartDate && delivery.LearnStartDate < LastInviableEndDate;
        }

        public bool IsAdultFunding(ILearningDelivery delivery)
        {
            return delivery.FundModel == TypeOfFunding.AdultSkills;
        }

        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery)
        {
            return It.Has(learner.DateOfBirthNullable) &&
                   (delivery.LearnStartDate - learner.DateOfBirthNullable.Value) >= MinimumViableAge;
        }

        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.FullyFundedLearningAim);
        }

        public bool IsEarlyStageNVQ(ILearningDelivery delivery)
        {
            var deliveries = _larsData.GetDeliveriesFor(delivery.LearnAimRef).AsSafeReadOnlyList();

            return deliveries
                .Any(x => It.IsInRange(x.NotionalNVQLevelv2, LARSNotionalNVQLevelV2.EntryLevel, LARSNotionalNVQLevelV2.Level1, LARSNotionalNVQLevelV2.Level2));
        }

        public bool IsExcluded(ILearner candidate)
        {
           /*
            This rule is not triggered by apprenticeships(DD07 = Y),                         <=IsApprenticeship
            traineeship non work prep/ experience(DD29 = Y),                                    <= IsInflexibleElementOfTrainingAim
            OLASS learners(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 034) or <= IsLearnerInCustody
            RoTL(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 328). <= IsReleasedOnTemporaryLicence
            It is also not triggered by learners on active benefits(DD28 = Y),  <= IsAdultFundedUnemployedWithBenefits
            Unemployed learners on other state benefits(DD21 = Y),               <= IsAdultFundedUnemployedWithOtherStateBenefits
            Restarts(LearningDeliveryFAM.LearnDelFAMType = RES),                   <= IsRestart
            English and maths basic skills(LARS_BasicSkillsType = 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35  <= IsBasicSkillsLearner
            (where LearningDelivery.LearnAimRef = LARS_LearnAimRef and LearningDelivery.LearnStartDate => LARS_EffectiveFrom
            (and =< LARS_EffectiveTo or LARS_EffectiveTo = null)),
            Providers with an organisation type of Specialist Designated College(LearningProvider.UKPRN = Org_Details.UKPRN and LegalOrgType = USDC),  <= IsLegalOrgTypeMatchForUkprn
            or
                Steel Industries Redundancy Training(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 347)  <= IsSteelWorkerRedundancyTraining
           */

            return
                    CheckLearningDeliveries(candidate, IsApprenticeship)
                || IsInflexibleElementOfTrainingAim(candidate)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsReleasedOnTemporaryLicence))
                || IsAdultFundedUnemployedWithBenefits(candidate)
                || IsAdultFundedUnemployedWithOtherStateBenefits(candidate)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsRestart))
                || CheckLearningDeliveries(candidate, IsBasicSkillsLearner)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsSteelWorkerRedundancyTraining) || IsLegalOrgTypeMatchForUkprn());
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (IsExcluded(objectToValidate))
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        public void ValidateDeliveries(ILearner learner)
        {
            var learnRefNumber = learner.LearnRefNumber;

            /* For starts after 31 July 2016 and before 1 August 2017
             ( LearningDelivery.LearnStartDate > 2016 - 07 - 31 and LearningDelivery.LearnStartDate < 2017 - 08 - 01),  <= for a delivery after the given date
             and adult skills funded(LearningDelivery.FundModel = 35),                                                    <= that is adult skills
             for aims that are level 2 or below(LARS_NotionalNVQLevelv2 = E, 1, or 2 where LearningDelivery.LearnAimRef = LARS_LearnAimRef), <= is NVQ V2 level 2 or below
                by learners aged 24 or over at the start of the aim and((LearningDelivery.LearnStartDate - Learner.DateOfBirth) >= 24 years),  <= in target age group
                full funding cannot be claimed

                (error where LearningDeliveryFAM.LearnDelFAMType = FFI and LearningDeliveryFAM.LearnDelFAMCode = 1) <= IsFullyFundedLearningAim
                */

            foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries
                .SafeWhere(x => IsViableStart(x)
                                && IsAdultFunding(x)
                                && IsTargetAgeGroup(learner, x)
                                && IsEarlyStageNVQ(x)
                                && x.LearningDeliveryFAMs != null))
            {
                foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (IsFullyFundedLearningAim(learningDeliveryFam))
                    {
                        HandleValidationError(learnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery, learningDeliveryFam, learner));
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery, ILearningDeliveryFAM learningDeliveryFam, ILearner learner)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDeliveryFam.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDeliveryFam.LearnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };
        }
    }
}
