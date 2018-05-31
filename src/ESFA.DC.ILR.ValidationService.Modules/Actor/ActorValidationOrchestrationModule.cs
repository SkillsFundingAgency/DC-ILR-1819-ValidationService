using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorValidationOrchestrationModule : BaseValidationOrchestrationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RuleSetOrchestrationService<ILearner, IValidationError>>().As<IRuleSetOrchestrationService<ILearner, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<RuleSetExecutionService<ILearner>>().As<IRuleSetExecutionService<ILearner>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService<IValidationError>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
