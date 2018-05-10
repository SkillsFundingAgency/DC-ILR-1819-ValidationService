using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ValidationServiceSfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ValidationOrchestrationSfModule>();
            builder.RegisterModule<DataModule>();
        }
    }
}
