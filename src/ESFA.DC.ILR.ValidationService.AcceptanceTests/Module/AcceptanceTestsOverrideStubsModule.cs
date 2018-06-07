using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class AcceptanceTestsOverrideStubsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<AcceptanceTestsExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
        }
    }
}
