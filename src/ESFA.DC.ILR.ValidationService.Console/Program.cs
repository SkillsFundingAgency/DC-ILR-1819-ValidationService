using System.Linq;
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
            var argsList = args.ToList();
            
            if (!argsList.Any())
            {
                argsList.Add(@"Files/ILR.xml");
            }

            RunValidation(argsList.First());
        }

        private static void RunValidation(string filePath)
        {
            var validationContext = new ValidationContextStub();

            validationContext.Input = filePath;

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
