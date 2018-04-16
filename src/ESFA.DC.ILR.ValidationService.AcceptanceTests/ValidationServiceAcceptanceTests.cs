using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;
//using DCT.TestDataGenerator.Functor;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTests
    {
        private static List<ILearnerMultiMutator> _functors = new List<ILearnerMultiMutator>(100);
        private static int UKPRN = 90000064;

        private void AddFunctor(ILearnerMultiMutator i)
        {
            _functors.Add(i);
        }

        [Theory]
        [InlineData("ULN_03",false)]
        //[InlineData("ULN_04", false)]
        //[InlineData("ULN_05", false)]
        //[InlineData("ULN_06", false)]
        public void TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            _functors = new List<ILearnerMultiMutator>(100);
            var cache = new DataCache();
            var rfp = new RuleToFunctorParser(cache);
            rfp.CreateFunctors(AddFunctor);
            XmlGenerator generator = new XmlGenerator(rfp, UKPRN);
            List<ActiveRuleValidity> rules = new List<ActiveRuleValidity>(100);
            uint scale = 1;
            rules.Add(new ActiveRuleValidity() { RuleName = rulename, Valid = valid });
            var result = generator.CreateAllXml(rules, scale, "ESFA/ILR/2018-19");
            Dictionary<string, string> files = generator.FileContent();
            foreach (var kvp in files)
            {
                var content = kvp.Value;
                var validationResult = RunValidation(content);
            }


            //_functors = new List<ILearnerMultiMutator>(100);
            //int UKPRN = 8;
            //var cache = new DataCache();
            //RuleToFunctorParser rfp = new RuleToFunctorParser(cache);
            //rfp.CreateFunctors(AddFunctor);
            //XmlGenerator generator = new XmlGenerator(rfp, UKPRN);
            //List<ActiveRuleValidity> rules = new List<ActiveRuleValidity>() { new ActiveRuleValidity() { RuleName = "ULN_03", Valid = false } }; ;            var result = generator.CreateAllXml(rules, 1, XmlGenerator.ESFA201819Namespace).ToList();

            //result.Count.Should().Be(_functors.Where(s => s.RuleName() == rules[0].RuleName).First().LearnerMutators(cache).ToList().Count());
        }

        private string RunValidation(string content)
        {
            var validationContext = new ValidationContextStub
            {
                Input = content,
                Output = null
            };

            var container = BuildContainer();

            using (var scope = container.BeginLifetimeScope(c => RegisterContext(c, validationContext)))
            {
                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                var result = ruleSetOrchestrationService.Execute(validationContext);
            }

            return validationContext.Output;
        }

        private void RegisterContext(ContainerBuilder containerBuilder, IValidationContext validationContext)
        {
            containerBuilder.RegisterInstance(validationContext).As<IValidationContext>();
        }

        private IContainer BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceAcceptanceTestsModule>();

            return containerBuilder.Build();
        }


        //private void AddFunctor(ILearnerMultiMutator i)
        //{
        //    _functors.Add(i);
        //}
    }


}
