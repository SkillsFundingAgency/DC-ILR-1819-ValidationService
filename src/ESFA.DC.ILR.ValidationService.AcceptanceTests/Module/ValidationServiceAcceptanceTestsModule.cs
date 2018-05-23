using Autofac;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.Modules.PreValidation;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<AcceptanceTestsOrchestrationModule>();
            builder.RegisterModule<PreValidationDataModule>();
            builder.RegisterModule<ActorDataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<AcceptanceTestsRuleSetModule>();
            builder.RegisterModule<AcceptanceTestsOverrideStubsModule>();
        }
    }
}
