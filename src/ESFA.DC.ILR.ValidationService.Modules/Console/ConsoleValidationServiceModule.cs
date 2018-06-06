using Autofac;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ConsoleValidationOrchestrationModule>();
            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<RuleSetModule>();
            builder.RegisterModule<ActorStubsModule>();

            builder.RegisterModule<DataServiceModule>();
            builder.RegisterModule<ConsoleCachePopulationModule>();
            builder.RegisterModule<CacheModule>();
        }
    }
}
