using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ConsoleValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ConsoleValidationOrchestrationModule>();
            builder.RegisterModule<PreValidationDataModule>();
            builder.RegisterModule<ActorDataModule>();
            builder.RegisterModule<ConsoleDataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<ConsoleRuleSetModule>();
        }
    }
}
