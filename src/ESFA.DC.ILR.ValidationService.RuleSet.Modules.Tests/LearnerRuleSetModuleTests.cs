using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Tests
{
    public class LearnerRuleSetModuleTests
    {
        /// <summary>
        /// Assembly and registration rule cardinality is correct.
        /// </summary>
        [Fact]
        public void AssemblyAndRegistrationRuleCardinalityIsCorrect()
        {
            var registeredItems = GetContainerRuleSet();
            var assemblyItems = GetAssemblyRuleSet();

            var issues = new List<object>();

            foreach (var ruleType in assemblyItems)
            {
                if (registeredItems.Where(x => x.GetType().Name == ruleType.Name).Count() != 1)
                {
                    issues.Add(ruleType);
                }
            }

            Assert.Empty(issues);
        }

        /// <summary>
        /// Assembly and the registration counts match.
        /// </summary>
        [Fact]
        public void AssemblyAndRegistrationCountsMatch()
        {
            var registeredItems = GetContainerRuleSet();
            var assemblyItems = GetAssemblyRuleSet();

            registeredItems.Should().HaveCount(assemblyItems.Count());
        }

        /// <summary>
        /// Gets the assembly rule set.
        /// </summary>
        /// <returns>a collection of <see cref="Type"/></returns>
        public IReadOnlyCollection<Type> GetAssemblyRuleSet()
        {
            var asm = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "ESFA.DC.ILR.ValidationService.Rules.dll"));
            return asm.GetTypes().Where(x => typeof(IRule<ILearner>).IsAssignableFrom(x)).ToList();
        }

        /// <summary>
        /// Gets the container rule set.
        /// </summary>
        /// <returns>a collection of <see cref="IRule{ILearner}"/></returns>
        public IReadOnlyCollection<IRule<ILearner>> GetContainerRuleSet()
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder);

            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<LearnerRuleSetModule>();

            var container = builder.Build();

            return container.Resolve<IEnumerable<IRule<ILearner>>>().ToList();
        }

        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Mock<IValidationErrorHandler>().Object).As<IValidationErrorHandler>();
        }
    }
}
