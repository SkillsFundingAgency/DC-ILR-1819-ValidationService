using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.PreValidation;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleValidationOrchestrationModule : ActorValidationOrchestrationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ConsolePreValidationOrchestrationService<IValidationError>>()
                .As<IPreValidationOrchestrationService<IValidationError>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ValidateXMLSchemaService>().As<IValidateXMLSchemaService>().InstancePerLifetimeScope();
            builder.RegisterType<SchemaFileContentStringProviderService>().As<ISchemaStringProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<FileSystemFileContentStringProviderService>().As<IMessageStreamProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageFileProviderService>().As<IValidationItemProviderService<IMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<RuleSetOrchestrationService<IMessage, IValidationError>>().As<IRuleSetOrchestrationService<IMessage, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<IMessage>>().As<IRuleSetResolutionService<IMessage>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<OneHundredThousandLearnerProvider>().As<IValidationItemProviderService<IEnumerable<ILearner>>>();
            builder.RegisterType<MessageProviderService>().As<IValidationItemProviderService<IEnumerable<IMessage>>>().InstancePerLifetimeScope();
        }
    }
}
