using Autofac;
using ESFA.DC.ILR.ValidationService.Modules.Console;

namespace ESFA.DC.ILR.ValidationService.Modules.Root
{
    public class ConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<SerializationModule>();
            builder.RegisterModule<CacheModule>();
            builder.RegisterModule<DataServiceModule>();

            builder.RegisterModule<ConsoleCachePopulationModule>();
        }
    }
}
