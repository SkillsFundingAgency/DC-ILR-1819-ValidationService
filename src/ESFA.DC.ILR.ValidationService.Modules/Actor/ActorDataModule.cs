using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorCachePopulationService>().As<IPopulationService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
