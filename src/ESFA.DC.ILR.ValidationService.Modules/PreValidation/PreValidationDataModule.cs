using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.PreValidation
{
    public class PreValidationDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationPopulationService>().As<IPreValidationPopulationService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AzureInternalDataCachePopulationServiceStub>()
                .As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
