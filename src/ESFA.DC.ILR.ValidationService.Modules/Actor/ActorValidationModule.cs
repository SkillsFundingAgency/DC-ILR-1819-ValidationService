﻿using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ActorValidationOrchestrationModule>();
            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<RuleSetModule>();

            builder.RegisterModule<DataServiceModule>();
            builder.RegisterModule<ActorStubsModule>();
        }
    }
}
