using System.Collections.Generic;
using System.Linq;
using Autofac;
using DCT.TestDataGenerator;
using DCT.TestDataGenerator.Functor;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTests
    {
        private const uint Scale = 1;

        [Theory]
        [InlineData("ULN_03", false)]
        public void TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            var learnersFound = new List<string>();
            var excludedLearnersFound = new List<string>();
            var unexpectedLearnersFound = new List<string>();

            var activeRuleValidities = new List<ActiveRuleValidity>()
            {
                new ActiveRuleValidity() { RuleName = rulename, Valid = valid }
            };

            var generator = BuildXmlGenerator();

            var expectedResult = generator.CreateAllXml(activeRuleValidities, Scale, XmlGenerator.ESFA201819Namespace).ToList();

            var fileContents = generator.FileContent().Values;

            foreach (var fileContent in fileContents)
            {
                var validationErrors = RunValidation(fileContent);

                foreach (var validationError in validationErrors)
                {
                    var fileRuleLearners = expectedResult.Where(s => s.LearnRefNumber == validationError.LearnerReferenceNumber).ToList();

                    if (!fileRuleLearners.Any())
                    {
                        unexpectedLearnersFound.Add(validationError.LearnerReferenceNumber);
                    }
                    else if (fileRuleLearners.First().ExclusionRecord)
                    {
                        excludedLearnersFound.Add(validationError.LearnerReferenceNumber);
                    }
                    else
                    {
                        learnersFound.Add(validationError.LearnerReferenceNumber);
                    }
                }

                excludedLearnersFound.Should().BeEmpty();
                unexpectedLearnersFound.Should().BeEmpty();
                learnersFound.Should().HaveCount(expectedResult.Count(s => !s.ExclusionRecord));
            }
        }

        private IEnumerable<IValidationError> RunValidation(string fileContent)
        {
            var validationContext = new ValidationContextStub
            {
                Input = fileContent
            };

            using (var scope = BuildContainer().BeginLifetimeScope(c => RegisterContext(c, validationContext)))
            {
                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                return ruleSetOrchestrationService.Execute(validationContext);
            }
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

        private XmlGenerator BuildXmlGenerator()
        {
            var functors = new List<ILearnerMultiMutator>();

            var ruleToFunctorParser = new RuleToFunctorParser(new DataCache());

            ruleToFunctorParser.CreateFunctors(f => functors.Add(f));

            return new XmlGenerator(ruleToFunctorParser, 90000064);
        }
    }
}
