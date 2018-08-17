using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Tests
{
    public class MessageRuleSetModuleTests
    {
        [Fact]
        public void MessageRuleSet()
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder);

            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<MessageRuleSetModule>();

            var container = builder.Build();

            var rules = container.Resolve<IEnumerable<IRule<IMessage>>>().ToList();

            rules.Should().ContainItemsAssignableTo<IRule<IMessage>>();

            var ruleTypes = new List<Type>()
            {
                typeof(UKPRN_03Rule)
            };

            foreach (var ruleType in ruleTypes)
            {
                rules.Should().ContainSingle(r => r.GetType() == ruleType);
            }

            rules.Should().HaveCount(1);
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<IValidationErrorHandler>().Object).As<IValidationErrorHandler>();
        }
    }
}
