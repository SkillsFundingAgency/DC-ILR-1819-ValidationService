using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Common;


namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationOrchestrationAcceptanceTestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<RuleSetOrchestrationService<ILearner, IValidationError>>().As<IRuleSetOrchestrationService<ILearner, IValidationError>>();
            //builder.RegisterType<AutoFacRuleSetResolutionServiceStub<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            //builder.RegisterType<RuleSetExecutionService<ILearner>>().As<IRuleSetExecutionService<ILearner>>();
            //builder.RegisterType<XmlSerializationService>().As<ISerializationService>();
            //builder.RegisterType<MessageFileSystemProviderServiceStub>().As<IValidationItemProviderService<IMessage>>();
            //builder.RegisterType<LearnerProviderServiceStub>().As<IValidationItemProviderService<IEnumerable<ILearner>>>();
            //builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            //builder.RegisterType<AcceptanceTestsValidationErrorHandlerOutputService>().As<IValidationOutputService<IValidationError>>();
        }
    }
}
