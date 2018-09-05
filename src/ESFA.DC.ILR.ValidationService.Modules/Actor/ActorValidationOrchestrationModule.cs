using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorValidationOrchestrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RuleSetOrchestrationService<ILearner, IValidationError>>().As<IRuleSetOrchestrationService<ILearner, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<IMessage>>().As<IRuleSetResolutionService<IMessage>>();
            builder.RegisterType<RuleSetExecutionService<ILearner>>().As<IRuleSetExecutionService<ILearner>>();
            builder.RegisterType<RuleSetExecutionService<IMessage>>().As<IRuleSetExecutionService<IMessage>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService<IValidationError>>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerProviderService>().As<IValidationItemProviderService<IEnumerable<ILearner>>>().InstancePerLifetimeScope();
        }
    }
}
