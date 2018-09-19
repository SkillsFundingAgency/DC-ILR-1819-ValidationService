using Autofac;

namespace ESFA.DC.ILR.ValidationService.Modules.Root
{
    public class ActorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<SerializationModule>();
            builder.RegisterModule<CacheModule>();
            builder.RegisterModule<DataServiceModule>();
        }
    }
}
