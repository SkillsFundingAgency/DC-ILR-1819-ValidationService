using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<PreValidationOrchestrationModule>();
            builder.RegisterModule<PreValidationDataModule>();
            builder.RegisterModule<MessageRuleSetModule>();
        }
    }
}
