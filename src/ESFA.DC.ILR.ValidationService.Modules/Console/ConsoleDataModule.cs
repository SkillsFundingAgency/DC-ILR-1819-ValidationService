using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InternalDataCachePopulationServiceStub>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
        }
    }
}
