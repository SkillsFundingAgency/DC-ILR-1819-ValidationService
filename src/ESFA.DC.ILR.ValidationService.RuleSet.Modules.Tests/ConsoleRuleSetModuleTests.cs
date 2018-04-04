using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimSeqNumber;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Tests
{
    public class ConsoleRuleSetModuleTests
    {
        [Fact]
        public void RuleSet()
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder);

            builder.RegisterModule<DataModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<ConsoleRuleSetModule>();

            var container = builder.Build();

            var rules = container.Resolve<IEnumerable<IRule<ILearner>>>().ToList();

            rules.Should().ContainItemsAssignableTo<IRule<ILearner>>();

            var ruleTypes = new List<Type>()
            {
                typeof(AddHours_01Rule),
                typeof(AddHours_02Rule),
                typeof(AddHours_04Rule),
                typeof(AddHours_05Rule),
                typeof(AddHours_06Rule),
                typeof(AimSeqNumber_02Rule),
            };

            foreach (var ruleType in ruleTypes)
            {
                rules.Should().ContainSingle(r => r.GetType() == ruleType);
            }

            rules.Should().HaveCount(6);
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<IValidationErrorHandler>().Object).As<IValidationErrorHandler>();
        }
    }
}
