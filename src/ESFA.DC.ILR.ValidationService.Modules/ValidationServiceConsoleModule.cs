using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ValidationServiceConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ValidationOrchestrationModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<ConsoleRuleSetModule>();
        }
    }
}
