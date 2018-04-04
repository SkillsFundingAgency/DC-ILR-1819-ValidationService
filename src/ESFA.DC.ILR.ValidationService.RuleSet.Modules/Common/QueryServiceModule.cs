using Autofac;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common
{
    public class QueryServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DateTimeQueryService>().As<IDateTimeQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryQueryService>().As<ILearningDeliveryQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerFAMQueryService>().As<ILearnerFAMQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryFAMQueryService>().As<ILearningDeliveryFAMQueryService>().InstancePerLifetimeScope();
        }
    }
}
