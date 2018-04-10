using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AutoFacRuleSetResolutionServiceStub<T> : IRuleSetResolutionService<T>
        where T : class
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutoFacRuleSetResolutionServiceStub(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IEnumerable<IRule<T>> Resolve()
        {
            var containerRules = _lifetimeScope.Resolve<IEnumerable<IRule<T>>>();

            var rules = containerRules.ToList();

            rules.AddRange(rules);
            rules.AddRange(rules);
            rules.AddRange(rules);
            rules.AddRange(rules);
            rules.AddRange(rules);
            rules.AddRange(rules);
            rules.AddRange(rules);

            return rules.Take(600);
        }
    }
}
