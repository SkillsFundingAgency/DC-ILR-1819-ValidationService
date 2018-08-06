using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.RuleSet;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ProviderModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LearnerProviderService>().As<IValidationItemProviderService<IEnumerable<ILearner>>>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
        }
    }
}
