using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class AcceptanceTestsOrchestrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContextMessageStringProviderService>().As<IMessageStringProviderService>();
            builder.RegisterType<MessageProviderService>().As<IValidationItemProviderService<IMessage>>();
            builder.RegisterType<ValidationErrorCachePassThroughOutputService>().As<IValidationOutputService<IValidationError>>();
            builder.RegisterType<RuleSetOrchestrationService<ILearner, IValidationError>>().As<IRuleSetOrchestrationService<ILearner, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<RuleSetExecutionService<ILearner>>().As<IRuleSetExecutionService<ILearner>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();
        }
    }
}
