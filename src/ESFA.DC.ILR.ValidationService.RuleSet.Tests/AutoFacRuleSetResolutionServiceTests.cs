using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class AutoFacRuleSetResolutionServiceTests
    {
        [Fact]
        public void Resolve_None()
        {
            var builder = new ContainerBuilder();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                service.Resolve().Should().BeEmpty();
            }
        }

        [Fact]
        public void Resolve_Single()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RuleOne>().As<IRule<string>>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                service.Resolve().First().Should().BeOfType<RuleOne>();
            }
        }

        [Fact]
        public void Resolve_Multiple()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RuleOne>().As<IRule<string>>();
            builder.RegisterType<RuleTwo>().As<IRule<string>>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                var rules = service.Resolve().ToList();

                rules.Should().AllBeAssignableTo<IRule<string>>();
                rules[0].Should().BeOfType<RuleOne>();
                rules[1].Should().BeOfType<RuleTwo>();
            }
        }

        [Fact]
        public void Resolve_NewScopeNewRule()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RuleOne>().As<IRule<string>>().InstancePerLifetimeScope();

            var container = builder.Build();

            IRule<string> ruleOne;
            IRule<string> ruleTwo;

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                ruleOne = service.Resolve().First();
            }

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                ruleTwo = service.Resolve().First();
            }

            ruleOne.Should().NotBeSameAs(ruleTwo);
        }

        private IRuleSetResolutionService<string> NewService(ILifetimeScope lifetimeScope)
        {
            return new AutoFacRuleSetResolutionService<string>(lifetimeScope);
        }
    }
}
