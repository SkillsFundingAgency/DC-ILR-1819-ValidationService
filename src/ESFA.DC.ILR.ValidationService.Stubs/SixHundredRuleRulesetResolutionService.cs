using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class SixHundredRuleRulesetResolutionService<T> : AutoFacRuleSetResolutionService<T>, IRuleSetResolutionService<T>
        where T : class
    {
        private const int RuleCount = 100;

        public SixHundredRuleRulesetResolutionService(ILifetimeScope lifetimeScope)
            : base(lifetimeScope)
        {
        }

        public IEnumerable<IRule<T>> Resolve()
        {
            var rulesList = base.Resolve().ToList();

            while (rulesList.Count < RuleCount)
            {
                rulesList.AddRange(rulesList);
            }

            return rulesList.Take(RuleCount);
        }
    }
}
