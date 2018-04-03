using System;
using System.Collections.Generic;
using Autofac;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract
{
    public abstract class AbstractRuleSetModule : Module
    {
        protected Type RuleSetType { get; set; }

        protected IEnumerable<Type> Rules { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var rule in Rules)
            {
                builder.RegisterType(rule).As(RuleSetType).InstancePerLifetimeScope();
            }
        }
    }
}
