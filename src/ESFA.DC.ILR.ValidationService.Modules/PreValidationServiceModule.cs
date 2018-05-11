using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class PreValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<PreValidationOrchestrationModule>();
            builder.RegisterModule<PreValidationDataModule>();
        }
    }
}
