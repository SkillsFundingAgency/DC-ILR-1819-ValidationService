using Autofac;

namespace ESFA.DC.ILR.ValidationService.Modules.PreValidation
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
