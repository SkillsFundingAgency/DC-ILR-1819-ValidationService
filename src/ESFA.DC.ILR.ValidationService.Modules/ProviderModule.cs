using System.Collections.Generic;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ProviderModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LearnerProviderService>().As<IValidationItemProviderService<IEnumerable<ILearner>>>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
        }
    }
}
