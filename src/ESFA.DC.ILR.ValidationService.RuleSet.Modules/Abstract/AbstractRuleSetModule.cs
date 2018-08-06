using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract
{
    public abstract class AbstractRuleSetModule : Module
    {
        protected Type RuleSetType { get; set; }

        protected IEnumerable<Type> Rules { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<QueryServiceModule>();

            foreach (var rule in Rules)
            {
                builder.RegisterType(rule).As(RuleSetType).InstancePerLifetimeScope();
            }
        }
    }
}
