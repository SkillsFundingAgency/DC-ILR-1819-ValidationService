using System.Linq;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Stubs;

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
            var validationContext = new ValidationContextStub
            {
                Input = filePath
            };

            var container = BuildContainer();

            using (var scope = container.BeginLifetimeScope(c => RegisterContext(c, validationContext)))
            {
                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();
                
                var result = ruleSetOrchestrationService.Execute(validationContext);
            }
        }

        private static void RegisterContext(ContainerBuilder containerBuilder, IValidationContext validationContext)
        {
            containerBuilder.RegisterInstance(validationContext).As<IValidationContext>();
        }

        private static IContainer BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            
            containerBuilder.RegisterModule<ValidationServiceConsoleModule>();

            return containerBuilder.Build();
        }
    }
}
