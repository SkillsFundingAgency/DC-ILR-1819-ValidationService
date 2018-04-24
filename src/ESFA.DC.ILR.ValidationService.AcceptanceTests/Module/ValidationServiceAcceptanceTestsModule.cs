using Autofac;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<AcceptanceTestsOrchestrationModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<AcceptanceTestsRuleSetModule>();
        }
    }
}
