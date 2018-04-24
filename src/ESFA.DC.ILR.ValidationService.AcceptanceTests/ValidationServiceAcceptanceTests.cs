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
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.AcceptanceTests
{
    public class ValidationServiceAcceptanceTests
    {
        private TestDataGeneratorFixture _generatureFixture;

        private List<string> _learnersFound;
        private List<string> _excludedLearnersFound;
        private List<string> _unexpectedLearnersFound;


        public ValidationServiceAcceptanceTests()
        {
        }

        [Theory]
        [InlineData("FundModel_01", false)]
        [InlineData("FundModel_03", false)]
        [InlineData("FundModel_04", false)]
        [InlineData("FundModel_05", false)]
        [InlineData("FundModel_06", false)]
        [InlineData("FundModel_07", false)]
        [InlineData("FundModel_08", false)]
        [InlineData("FundModel_09", false)]
        public void TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            _generatureFixture = new TestDataGeneratorFixture();
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
            int totalExpectedRows = expectedResult.Where(s => !s.ExclusionRecord).Sum(s => s.InvalidLines);
            _learnersFound.Should().HaveCount(totalExpectedRows);
        }

        private void PopulateResultsCollectionsBasedOnResults(IEnumerable<FileRuleLearner> expectedResult, IEnumerable<IValidationError> fileValidationResult)
        {
            foreach (var val in fileValidationResult)
            {
                var exclusionRecordFound = expectedResult.Count(s => s.ExclusionRecord && s.RuleName == val.RuleName && s.LearnRefNumber == val.LearnerReferenceNumber) > 0;
                if (exclusionRecordFound)
                {
                    _excludedLearnersFound.Add(val.LearnerReferenceNumber);
                }
                else
                {
                    var completelyExpectedResult = expectedResult.Count(s => s.RuleName == val.RuleName && s.LearnRefNumber == val.LearnerReferenceNumber) > 0;

                    if (completelyExpectedResult)
                    {
                        _learnersFound.Add(val.LearnerReferenceNumber);
                    }
                    else
                    {
                        var correctRule = expectedResult.Count(s => s.RuleName == val.RuleName) > 0;
                        if (correctRule)
                        {
                            _unexpectedLearnersFound.Add(val.LearnerReferenceNumber);
                        }
                    }
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
