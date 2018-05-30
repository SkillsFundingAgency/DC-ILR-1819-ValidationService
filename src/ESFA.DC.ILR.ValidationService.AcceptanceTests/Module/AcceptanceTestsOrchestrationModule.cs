using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class AcceptanceTestsOrchestrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageStringProviderService>().As<IValidationItemProviderService<IMessage>>();
            builder.RegisterType<ValidationErrorHandlerOutputService>().As<IValidationOutputService<IValidationError>>();
            builder.RegisterType<RuleSetOrchestrationService<ILearner, IValidationError>>().As<IRuleSetOrchestrationService<ILearner, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionServiceStub<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<RuleSetExecutionService<ILearner>>().As<IRuleSetExecutionService<ILearner>>();
            builder.RegisterType<XmlSerializationService>().As<ISerializationService>();
            builder.RegisterType<LearnerProviderServiceStub>().As<IValidationItemProviderService<IEnumerable<ILearner>>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();
        }
    }
}
