using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ActorValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ActorValidationOrchestrationModule>();
            builder.RegisterModule<ActorDataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<ConsoleRuleSetModule>();
        }
    }
}
