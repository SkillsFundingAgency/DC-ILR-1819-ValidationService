using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.Serialization.Xml;
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

        [Theory]
        [InlineData("FworkCode_05", false)]
        public async Task TestDataGenerator_ValidationServiceMatchesTestDataExpected(string rulename, bool valid)
        {
            var rules = new List<ActiveRuleValidity>(100)
            {
                new ActiveRuleValidity() { RuleName = rulename, Valid = valid }
            };

            var generator = BuildXmlGenerator();
            var expectedResult = generator.CreateAllXml(rules, Scale, XmlGenerator.ESFA201819Namespace);
            var files = generator.FileContent().Values;

            CreateResultsCollections();

            foreach (var content in files)
            {
                var fileValidationResult = RunValidation(content);
                PopulateResultsCollectionsBasedOnResults(expectedResult, await fileValidationResult);
            }

            _excludedLearnersFound.Should().BeEmpty();
            _unexpectedLearnersFound.Should().BeEmpty();
            var totalExpectedRows = expectedResult.Where(s => !s.ExclusionRecord).Sum(s => s.InvalidLines);
            _learnersFound.Should().HaveCount(totalExpectedRows);
        }

        private XmlGenerator BuildXmlGenerator()
        {
            var cache = new DataCache();
            var rfp = new RuleToFunctorParser(cache);

            rfp.CreateFunctors(null);

            return new XmlGenerator(rfp, Ukprn);
        }

        private void PopulateResultsCollectionsBasedOnResults(IEnumerable<FileRuleLearner> expectedResult, IEnumerable<IValidationError> fileValidationResult)
        {
            expectedResult = expectedResult.ToList();

            foreach (var val in fileValidationResult)
            {
                var exclusionRecordFound = expectedResult.Any(s =>
                                               s.ExclusionRecord && s.RuleName == val.RuleName &&
                                               s.LearnRefNumber == val.LearnerReferenceNumber);
                if (exclusionRecordFound)
                {
                    _excludedLearnersFound.Add(val.LearnerReferenceNumber);
                }
                else
                {
                    var completelyExpectedResult = expectedResult.Any(s =>
                                                       s.RuleName == val.RuleName &&
                                                       s.LearnRefNumber == val.LearnerReferenceNumber);

                    if (completelyExpectedResult)
                    {
                        _learnersFound.Add(val.LearnerReferenceNumber);
                    }
                    else
                    {
                        var correctRule = expectedResult.Any(s => s.RuleName == val.RuleName);
                        if (correctRule)
                        {
                            _unexpectedLearnersFound.Add(val.LearnerReferenceNumber);
                        }
                    }
                }
            }
        }

        private void CreateResultsCollections()
        {
            _learnersFound = new List<string>();
            _excludedLearnersFound = new List<string>();
            _unexpectedLearnersFound = new List<string>();
        }

        private async Task<IEnumerable<IValidationError>> RunValidation(string messageString)
        {
            var serializationService = new XmlSerializationService();

            var validationContext = new ValidationContext
            {
                Input = serializationService.Deserialize<Message>(messageString)
            };

            var preValidationContext = new PreValidationContext()
            {
                Input = messageString
            };

            var container = BuildContainer();

            using (var scope = container.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(validationContext).As<IValidationContext>();
                c.RegisterInstance(preValidationContext).As<IPreValidationContext>();
            }))
            {
                var errorLookupPopulationService = scope.Resolve<IErrorLookupPopulationService>();

                await errorLookupPopulationService.PopulateAsync(CancellationToken.None);

                var preValidationPopulationService = scope.Resolve<IPopulationService>();

                await preValidationPopulationService.PopulateAsync(CancellationToken.None);

                var ruleSetOrchestrationService = scope.Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                return await ruleSetOrchestrationService.ExecuteAsync(new List<string>(), CancellationToken.None);
            }
        }

        private IContainer BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceAcceptanceTestsModule>();

            return containerBuilder.Build();
        }
    }
}
