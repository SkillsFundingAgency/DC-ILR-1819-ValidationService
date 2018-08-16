using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleValidationOrchestrationModule : ActorValidationOrchestrationModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ConsolePreValidationOrchestrationService<ILearner, IValidationError>>()
                .As<IPreValidationOrchestrationService<IValidationError>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MessageValidationServiceStub<IValidationError>>().As<IMessageValidationService<IValidationError>>().InstancePerLifetimeScope();
            builder.RegisterType<FileSystemFileContentStringProviderService>().As<IMessageStringProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageFileProviderService>().As<IValidationItemProviderService<IMessage>>().InstancePerLifetimeScope();

            builder.RegisterType<SixHundredRuleRulesetResolutionService<ILearner>>().As<IRuleSetResolutionService<ILearner>>();
            builder.RegisterType<OneHundredThousandLearnerProvider>().As<IValidationItemProviderService<IEnumerable<ILearner>>>();
        }
    }
}
