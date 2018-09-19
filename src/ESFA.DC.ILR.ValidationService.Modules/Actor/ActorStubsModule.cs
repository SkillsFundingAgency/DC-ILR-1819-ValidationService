using Autofac;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorStubsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FCSDataServiceStub>().As<IFCSDataService>().InstancePerLifetimeScope();
        }
    }
}
