﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.HE.ELQ;
using ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE;
using ESFA.DC.ILR.ValidationService.Rules.HE.NUMHUS;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS;
using ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName;
using ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PMUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrevUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Learner.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimSeqNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OtherFundAdj;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Tests
{
    public class RuleSetModuleTests
    {
        [Fact]
        public void RuleSet()
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder);

            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<RuleSetModule>();

            var container = builder.Build();

            var rules = container.Resolve<IEnumerable<IRule<ILearner>>>().ToList();

            rules.Should().ContainItemsAssignableTo<IRule<ILearner>>();

            var ruleTypes = new List<Type>()
            {
                typeof(AchDate_02Rule),
                typeof(AchDate_03Rule),
                typeof(AchDate_04Rule),
                typeof(AchDate_05Rule),
                typeof(AchDate_07Rule),
                typeof(AchDate_09Rule),
                typeof(AchDate_10Rule),
                typeof(AddHours_01Rule),
                typeof(AddHours_02Rule),
                typeof(AddHours_04Rule),
                typeof(AddHours_05Rule),
                typeof(AddHours_06Rule),
                typeof(AFinType_10Rule),
                typeof(AimSeqNumber_02Rule),
                typeof(AimType_01Rule),
                typeof(AimType_05Rule),
                typeof(AimType_07Rule),
                typeof(CompStatus_01Rule),
                typeof(CompStatus_02Rule),
                typeof(CompStatus_03Rule),
                typeof(CompStatus_04Rule),
                typeof(CompStatus_05Rule),
                typeof(CompStatus_06Rule),
                typeof(ConRefNumber_01Rule),
                typeof(DateOfBirth_01Rule),
                typeof(DateOfBirth_02Rule),
                typeof(DateOfBirth_03Rule),
                typeof(DateOfBirth_04Rule),
                typeof(DateOfBirth_05Rule),
                typeof(DateOfBirth_06Rule),
                typeof(DateOfBirth_07Rule),
                typeof(DateOfBirth_12Rule),
                typeof(DateOfBirth_13Rule),
                typeof(DateOfBirth_14Rule),
                typeof(DateOfBirth_20Rule),
                typeof(DateOfBirth_23Rule),
                typeof(DateOfBirth_24Rule),
                typeof(DateOfBirth_25Rule),
                typeof(DateOfBirth_26Rule),
                typeof(DateOfBirth_27Rule),
                typeof(DateOfBirth_28Rule),
                typeof(DateOfBirth_29Rule),
                typeof(DateOfBirth_30Rule),
                typeof(DateOfBirth_32Rule),
                typeof(DateOfBirth_35Rule),
                typeof(DateOfBirth_36Rule),
                typeof(DateOfBirth_38Rule),
                typeof(DateOfBirth_39Rule),
                typeof(DateOfBirth_46Rule),
                typeof(DateOfBirth_47Rule),
                typeof(DateOfBirth_48Rule),
                typeof(DateOfBirth_53Rule),
                typeof(ConRefNumber_03Rule),
                typeof(DelLocPostCode_03Rule),
                typeof(DelLocPostCode_11Rule),
                typeof(DelLocPostCode_16Rule),
                typeof(ELQ_01Rule),
                typeof(EmpOutcome_01Rule),
                typeof(EmpOutcome_02Rule),
                typeof(EmpOutcome_03Rule),
                typeof(EmpStat_08Rule),
                typeof(EmpStat_12Rule),
                typeof(FamilyName_01Rule),
                typeof(FamilyName_02Rule),
                typeof(FamilyName_04Rule),
                typeof(FundModel_01Rule),
                typeof(FundModel_03Rule),
                typeof(FundModel_04Rule),
                typeof(FundModel_05Rule),
                typeof(FundModel_06Rule),
                typeof(FundModel_07Rule),
                typeof(FundModel_08Rule),
                typeof(FundModel_08Rule),
                typeof(FundModel_09Rule),
                typeof(FworkCode_01Rule),
                typeof(FworkCode_02Rule),
                typeof(FworkCode_05Rule),
                typeof(GivenNames_01Rule),
                typeof(GivenNames_02Rule),
                typeof(GivenNames_04Rule),
                typeof(LearnActEndDate_01Rule),
                typeof(LearnActEndDate_04Rule),
                typeof(LearnAimRef_01Rule),
                typeof(LearnAimRef_29Rule),
                typeof(LearnAimRef_30Rule),
                typeof(LearnAimRef_55Rule),
                typeof(LearnAimRef_56Rule),
                typeof(LearnAimRef_57Rule),
                typeof(LearnAimRef_59Rule),
                typeof(LearnAimRef_80Rule),
                typeof(LearnDelFAMDateFrom_01Rule),
                typeof(LearnDelFAMDateFrom_02Rule),
                typeof(LearnDelFAMDateFrom_03Rule),
                typeof(LearnDelFAMDateFrom_04Rule),
                typeof(LearnDelFAMDateTo_01Rule),
                typeof(LearnDelFAMDateTo_02Rule),
                typeof(LearnDelFAMDateTo_03Rule),
                typeof(LearnDelFAMDateTo_04Rule),
                typeof(LearnDelFAMType_02Rule),
                typeof(LearnDelFAMType_03Rule),
                typeof(LearnDelFAMType_39Rule),
                typeof(LearnDelFAMType_44Rule),
                typeof(LearnFAMType_16Rule),
                typeof(LLDDCat_01Rule),
                typeof(LLDDCat_02Rule),
                typeof(MathGrade_03Rule),
                typeof(NETFEE_01Rule),
                typeof(NETFEE_02Rule),
                typeof(NUMHUS_01Rule),
                typeof(OtherFundAdj_01Rule),
                typeof(OutGrade_03Rule),
                typeof(PartnerUKPRN_01Rule),
                typeof(PartnerUKPRN_02Rule),
                typeof(PartnerUKPRN_03Rule),
                typeof(PCFLDCS_02Rule),
                typeof(PlanLearnHours_01Rule),
                typeof(PlanLearnHours_02Rule),
                typeof(PlanLearnHours_03Rule),
                typeof(PlanLearnHours_04Rule),
                typeof(PMUKPRN_01Rule),
                typeof(Postcode_14Rule),
                typeof(PostcodePrior_01Rule),
                typeof(PrevUKPRN_01Rule),
                typeof(PrimaryLLDD_02Rule),
                typeof(PrimaryLLDD_01Rule),
                typeof(PrimaryLLDD_02Rule),
                typeof(PrimaryLLDD_03Rule),
                typeof(PrimaryLLDD_04Rule),
                typeof(PriorAttain_01Rule),
                typeof(PriorAttain_02Rule),
                typeof(PriorLearnFundAdj_01Rule),
                typeof(QUALENT3_01Rule),
                typeof(QUALENT3_02Rule),
                typeof(R07Rule),
                typeof(R85Rule),
                typeof(TTACCOM_04Rule),
                typeof(UKPRN_06Rule),
                typeof(ULN_02Rule),
                typeof(ULN_03Rule),
                typeof(ULN_04Rule),
                typeof(ULN_05Rule),
                typeof(ULN_06Rule),
                typeof(ULN_07Rule),
                typeof(ULN_09Rule),
                typeof(ULN_10Rule),
                typeof(ULN_12Rule),
            };

            foreach (var ruleType in ruleTypes)
            {
                rules.Should().ContainSingle(r => r.GetType() == ruleType);
            }

            rules.Should().HaveCount(144);
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<IValidationErrorHandler>().Object).As<IValidationErrorHandler>();
        }
    }
}
