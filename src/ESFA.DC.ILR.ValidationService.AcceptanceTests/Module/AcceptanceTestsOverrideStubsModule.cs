using Autofac;
using ESFA.DC.ILR.ValidationService.AcceptanceTests.Stubs;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class AcceptanceTestsOverrideStubsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<AcceptanceTestsValidationErrorsCachePopulationServiceStub>().As<IErrorLookupPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<AcceptanceTestsExternalDataCachePopulationServiceStub>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationService>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationService>().As<ICreateInternalDataCache>().InstancePerLifetimeScope();
        }
    }
}
