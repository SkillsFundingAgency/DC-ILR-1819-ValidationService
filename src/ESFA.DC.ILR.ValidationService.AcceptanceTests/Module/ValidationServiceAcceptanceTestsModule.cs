using Autofac;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<AcceptanceTestsOrchestrationModule>();
            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<AcceptanceTestsRuleSetModule>();
            builder.RegisterModule<AcceptanceTestsOverrideStubsModule>();
            builder.RegisterModule<SerializationModule>();
            builder.RegisterModule<ProviderModule>();
        }
    }
}
