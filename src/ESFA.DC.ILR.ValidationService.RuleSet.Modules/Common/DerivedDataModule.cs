using Autofac;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common
{
    internal class DerivedDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DD01>().As<IDD01>().InstancePerLifetimeScope();
            builder.RegisterType<DD04>().As<IDD04>().InstancePerLifetimeScope();
            builder.RegisterType<DD06>().As<IDD06>().InstancePerLifetimeScope();
            builder.RegisterType<DD07>().As<IDD07>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_21Rule>().As<IDerivedData_21Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_28Rule>().As<IDerivedData_28Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_29Rule>().As<IDerivedData_29Rule>().InstancePerLifetimeScope();
        }
    }
}
