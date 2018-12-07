using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_15RuleTests : AbstractRuleTests<ESMType_15Rule>
    {
        private readonly string[] _esmTypes = { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ESMType_15");
        }

        [Fact]
        public void ConditionMet_True()
        {
            string[] duplicateTypes = { "SEI", "EII" };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>(), _esmTypes))
                .Returns(true);

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.GetDuplicatedEmploymentStatusMonitoringTypesForTypes(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>(), _esmTypes))
                .Returns(duplicateTypes);

            NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object).ConditionMet(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>(), _esmTypes))
                .Returns(false);

            NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object).ConditionMet(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>()).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>()
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "SEI"
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "SEI"
                            }
                        }
                    }
                }
            };

            var employmentStatusMonitorings = learner.LearnerEmploymentStatuses.SelectMany(les => les.EmploymentStatusMonitorings);

            string[] duplicateTypes = { "SEI" };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, _esmTypes))
                .Returns(true);

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, _esmTypes))
                .Returns(duplicateTypes);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>()
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "SEI"
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LOU"
                            }
                        }
                    }
                }
            };

            var employmentStatusMonitorings = learner.LearnerEmploymentStatuses.SelectMany(les => les.EmploymentStatusMonitorings);

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(qs => qs.HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, _esmTypes))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ESMType", "SEI, LOU")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("SEI, LOU");

            validationErrorHandlerMock.Verify();
        }

        private ESMType_15Rule NewRule(
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new ESMType_15Rule(learnerEmploymentStatusMonitoringQueryService, validationErrorHandler);
        }
    }
}
