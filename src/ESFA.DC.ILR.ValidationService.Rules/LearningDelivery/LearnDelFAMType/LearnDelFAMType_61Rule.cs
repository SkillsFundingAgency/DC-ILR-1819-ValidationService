using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_61Rule : AbstractRule, IRule<ILearner>
    {
        public const string MessagePropertyName = "LearnDelFAMType";
        public const string Name = "LearnDelFAMType_61";
        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;

        public LearnDelFAMType_61Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_61)
        {
            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
        }

        public DateTime LastInviableDate => new DateTime(2017, 07, 31);

        public TimeSpan MinimumViableAge => new TimeSpan(6940, 0, 0, 0); // 19 years

        public TimeSpan MaximumViableAge => new TimeSpan(8401, 0, 0, 0); // 23 years

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);
        }

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);
        }

        public bool IsRestart(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);
        }

        public bool IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining)
                || It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.InReceiptOfLowWages);
        }

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return validities.Any(x => x.IsCurrent(delivery.LearnStartDate))
                && annualValues.Any(IsBasicSkillsLearner);
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor)
        {
            return It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);
        }

        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearner candidate)
        {
            return _derivedData21.IsAdultFundedUnemployedWithOtherStateBenefits(candidate);
        }

        public bool IsAdultFundedUnemployedWithBenefits(ILearner candidate)
        {
            return _derivedData28.IsAdultFundedUnemployedWithBenefits(candidate);
        }

        public bool IsInflexibleElementOfTrainingAim(ILearner candidate)
        {
            return _derivedData29.IsInflexibleElementOfTrainingAim(candidate);
        }

        public bool IsApprenticeship(ILearningDelivery delivery)
        {
            return _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);
        }

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition)
        {
            return delivery.LearningDeliveryFAMs.SafeAny(matchCondition);
        }

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition)
        {
            return candidate.LearningDeliveries.SafeAny(matchCondition);
        }

        public bool IsAdultFunding(ILearningDelivery delivery)
        {
            return delivery.FundModel == TypeOfFunding.AdultSkills;
        }

        public bool IsViableStart(ILearningDelivery delivery)
        {
            return delivery.LearnStartDate > LastInviableDate;
        }

        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery)
        {
            return It.Has(learner.DateOfBirthNullable) && It.IsBetween(
                       delivery.LearnStartDate - learner.DateOfBirthNullable.Value, MinimumViableAge, MaximumViableAge);
        }

        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor)
        {
            return It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.FullyFundedLearningAim);
        }

        public bool IsLevel2Nvq(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsV2NotionalLevel2(larsDelivery);
        }

        public bool IsNotEntitled(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return !larsDelivery.Categories.SafeAny(category => category.CategoryRef == TypeOfLARSCategory.LegalEntitlementLevel2);
        }

        public bool IsV2NotionalLevel2(ILARSLearningDelivery delivery)
        {
            return delivery.NotionalNVQLevelv2 == LARSNotionalNVQLevelV2.Level2;
        }

        public bool IsExcluded(ILearner candidate)
        {
            /* This rule is not triggered by apprenticeships(DD07 = Y),               <=IsApprenticeship
             traineeship non work prep/ experience(DD29 = Y),                         <= IsInflexibleElementOfTrainingAim
             OLASS learners(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 034) or  <= IsLearnerInCustody
             RoTL(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 328).  <= IsReleasedOnTemporaryLicence
             It is also not triggered by learners on active benefits(DD28 = Y),       <= IsAdultFundedUnemployedWithBenefits
             Unemployed learners on other state benefits(DD21 = Y),                   <= IsAdultFundedUnemployedWithOtherStateBenefits
             Restarts(LearningDeliveryFAM.LearnDelFAMType = RES),                     <= IsRestart
             English and maths basic skills(LARS_BasicSkillsType = 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35), <= IsBasicSkillsLearner
            (where LearningDelivery.LearnAimRef = LARS_LearnAimRef and LearningDelivery.LearnStartDate => LARS_EffectiveFrom
            (and =< LARS_EffectiveTo or LARS_EffectiveTo = null)),
            Providers with
            Steel Industries Redundancy Training(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 347),   <=IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages
            Learners in receipt of low wages(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 363)
            */

            return IsAdultFundedUnemployedWithOtherStateBenefits(candidate)
                || IsAdultFundedUnemployedWithBenefits(candidate)
                || IsInflexibleElementOfTrainingAim(candidate)
                || CheckLearningDeliveries(candidate, IsApprenticeship)
                || CheckLearningDeliveries(candidate, IsBasicSkillsLearner)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsReleasedOnTemporaryLicence))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsRestart))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages));
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
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

            /*
              LearningDelivery.LearnStartDate > 2017 - 07 - 31                                                  <= for a delivery after the given date
              and LearningDelivery.FundModel = 35                                                               <= that is adult skills
              and  ((LearningDelivery.LearnStartDate - Learner.DateOfBirth) >= 19 years and <= 23 years)        <= in target age group
                        and ((LearningDelivery.LearnAimRef = LARS_LearnAimRef and LARS_NotionalNVQLevelv2 = 2)  <= is NVQ V2 level 2
                            and (LearningDeliveryFAM.LearnDelFAMType = FFI and LearningDeliveryFAM.LearnDelFAMCode = 1)  <= IsFullyFundedLearningAim
                            and LARS_CategoryRef<> 37)

             */

            foreach (var learningDelivery in learner.LearningDeliveries
                .SafeWhere(x => IsViableStart(x)
                                && IsAdultFunding(x)
                                && IsTargetAgeGroup(learner, x)
                                && IsLevel2Nvq(x)
                                && IsNotEntitled(x)
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
