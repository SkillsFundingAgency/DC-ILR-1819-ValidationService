using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Modules.Stubs;

namespace ESFA.DC.ILR.ValidationService.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RunValidation();
        }

        private static void RunValidation()
        {
            var validationContext = new ValidationContextStub();

            var container = BuildContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                var result = ruleSetOrchestrationService.Execute(validationContext);
            }
        }

        private static IContainer BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceConsoleModule>();

            return containerBuilder.Build();
        }
    }
}
