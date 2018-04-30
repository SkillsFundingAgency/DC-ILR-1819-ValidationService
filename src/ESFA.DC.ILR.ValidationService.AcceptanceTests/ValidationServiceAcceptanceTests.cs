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
    public class ValidationServiceAcceptanceTests
    {
        private const uint Scale = 1;
        private const int Ukprn = 90000064;

        private List<string> _learnersFound;
        private List<string> _excludedLearnersFound;
        private List<string> _unexpectedLearnersFound;

        public ValidationServiceAcceptanceTests()
        {
        }

        [Theory]
        [InlineData("FworkCode_05", false)]
        public void TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            List<ActiveRuleValidity> rules = new List<ActiveRuleValidity>(100);
            rules.Add(new ActiveRuleValidity() { RuleName = rulename, Valid = valid });
            var generator = BuildXmlGenerator();
            var expectedResult = generator.CreateAllXml(rules, Scale, XmlGenerator.ESFA201819Namespace);
            var files = generator.FileContent().Values;

            CreateResultsCollections(expectedResult.Count());

            foreach (var content in files)
            {
                var fileValidationResult = RunValidation(content);
                PopulateResultsCollectionsBasedOnResults(expectedResult, fileValidationResult);
            }

            _excludedLearnersFound.Should().BeEmpty();
            _unexpectedLearnersFound.Should().BeEmpty();
            int totalExpectedRows = expectedResult.Where(s => !s.ExclusionRecord).Sum(s => s.InvalidLines);
            _learnersFound.Should().HaveCount(totalExpectedRows);
        }

        private XmlGenerator BuildXmlGenerator()
        {
            DataCache cache = new DataCache();
            RuleToFunctorParser rfp = new RuleToFunctorParser(cache);
            rfp.CreateFunctors(null);
            return new XmlGenerator(rfp, Ukprn);
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
