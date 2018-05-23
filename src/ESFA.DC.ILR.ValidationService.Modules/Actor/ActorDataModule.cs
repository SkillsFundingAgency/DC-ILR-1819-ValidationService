using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorPreValidationPopulationService>().As<IPreValidationPopulationService<IValidationContext>>().InstancePerLifetimeScope();

            builder.RegisterType<MessageCacheWithDataPopulationService>().As<IMessageCacheWithDataPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<InternalDataCacheWithDataPopulationserviceStub>().As<IInternalDataCacheWithDataPopulationService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
