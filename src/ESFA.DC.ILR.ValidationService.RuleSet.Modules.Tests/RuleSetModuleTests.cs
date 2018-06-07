﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
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
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;
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
                typeof(AddHours_01Rule),
                typeof(AddHours_02Rule),
                typeof(AddHours_04Rule),
                typeof(AddHours_05Rule),
                typeof(AddHours_06Rule),
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
                typeof(ConRefNumber_03Rule),
                typeof(DelLocPostCode_03Rule),
                typeof(DelLocPostCode_11Rule),
                typeof(DelLocPostCode_16Rule),
                typeof(EmpOutcome_01Rule),
                typeof(EmpOutcome_02Rule),
                typeof(EmpOutcome_03Rule),
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
                typeof(LearnActEndDate_01Rule),
                typeof(LearnActEndDate_04Rule),
                typeof(LearnAimRef_01Rule),
                typeof(LearnAimRef_29Rule),
                typeof(LearnAimRef_30Rule),
                typeof(LearnAimRef_55Rule),
                typeof(LearnAimRef_80Rule),
                typeof(ULN_03Rule),
            };

            foreach (var ruleType in ruleTypes)
            {
                rules.Should().ContainSingle(r => r.GetType() == ruleType);
            }

            rules.Should().HaveCount(44);
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<IValidationErrorHandler>().Object).As<IValidationErrorHandler>();
        }
    }
}