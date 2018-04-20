using System.Collections.Generic;
using System.Linq;
using Autofac;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stubs;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTests : IClassFixture<TestDataGeneratorFixture>
    {
        private TestDataGeneratorFixture _generatureFixture;

        private List<string> _learnersFound;
        private List<string> _excludedLearnersFound;
        private List<string> _unexpectedLearnersFound;


        public ValidationServiceAcceptanceTests(TestDataGeneratorFixture generatureFixture)
        {
            _generatureFixture = generatureFixture;
        }

        [Theory]
        [InlineData("ULN_03", false)]
        [InlineData("ULN_03", false)]
        [InlineData("ULN_03", false)]
        [InlineData("ULN_03", false)]
        [InlineData("ULN_03", false)]
        [InlineData("ULN_03", false)]
        //[InlineData("ULN_04", false)]
        //[InlineData("ULN_05", false)]
        //[InlineData("ULN_06", false)]
        public void TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            List<ActiveRuleValidity> rules = new List<ActiveRuleValidity>(100);
            uint scale = 1;
            rules.Add(new ActiveRuleValidity() { RuleName = rulename, Valid = valid });
            var expectedResult = _generatureFixture.Generator.CreateAllXml(rules, scale, XmlGenerator.ESFA201819Namespace);
            Dictionary<string, string> files = _generatureFixture.Generator.FileContent();

            CreateResultsCollections(expectedResult.Count());

            foreach (var kvp in files)
            {
                var content = kvp.Value;
                var fileValidationResult = RunValidation(content);
                PopulateResultsCollectionsBasedOnResults(expectedResult, fileValidationResult);
            }

            _excludedLearnersFound.Should().BeEmpty();
            _unexpectedLearnersFound.Should().BeEmpty();
            _learnersFound.Should().HaveCount(expectedResult.Count(s => !s.ExclusionRecord));
        }

        private void PopulateResultsCollectionsBasedOnResults(IEnumerable<FileRuleLearner> expectedResult, IEnumerable<IValidationError> fileValidationResult)
        {
            foreach (var val in fileValidationResult)
            {
                var row = expectedResult.Where(s => s.LearnRefNumber == val.LearnerReferenceNumber).ToList();

                if (!row.Any())
                {
                    _unexpectedLearnersFound.Add(val.LearnerReferenceNumber);
                }
                else if (row.First().ExclusionRecord)
                {
                    _excludedLearnersFound.Add(val.LearnerReferenceNumber);
                }
                else
                {
                    _learnersFound.Add(val.LearnerReferenceNumber);
                }
            }
        }

        private void CreateResultsCollections(int count)
        {
            _learnersFound = new List<string>(count);
            _excludedLearnersFound = new List<string>(count);
            _unexpectedLearnersFound = new List<string>(count);
        }

        private IEnumerable<IValidationError> RunValidation(string fileContent)
        {
            var validationContext = new ValidationContextStub
            {
                Input = fileContent,
                Output = null
            };

            var container = BuildContainer();

            IEnumerable<IValidationError> result = null;

            using (var scope = container.BeginLifetimeScope(c => RegisterContext(c, validationContext)))
            {
                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                result = ruleSetOrchestrationService.Execute(validationContext);
            }

            return result;
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
    }
}
