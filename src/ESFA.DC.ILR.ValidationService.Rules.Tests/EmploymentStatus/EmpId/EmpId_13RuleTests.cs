using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_13RuleTests : AbstractRuleTests<EmpId_13Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpId_13");
        }

        [Fact]
        public void EmpIdConditionMet_True()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = 999999999
                },
            };

            NewRule().EmpIdConditionMet(learnerEmploymentStatuses).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(123456789)]
        [InlineData(0)]
        public void EmpIdConditionMet_False(int? empId)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = empId
                }
            };

            NewRule().EmpIdConditionMet(learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 2;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(50)]
        [InlineData(null)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var aimType = 1;

            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void AimTypeConditionMet_False(int aimType)
        {
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Theory]
        [InlineData(61)]
        [InlineData(100)]
        public void LearnStartDateConditionMet_True(int days)
        {
            var filePrepDate = DateTime.Today;

            var learnStartDate = filePrepDate.AddDays(-days);

            NewRule().LearnStartDateConditionMet(learnStartDate, filePrepDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(59)]
        [InlineData(1)]
        [InlineData(0)]
        public void LearnStartDateConditionMet_False(int days)
        {
            var filePrepDate = DateTime.Today;

            var learnStartDate = filePrepDate.AddDays(-days);

            NewRule().LearnStartDateConditionMet(learnStartDate, filePrepDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 2;
            var aimType = 1;
            var learnStartDate = new DateTime(2018, 7, 1);
            var filePrepDate = new DateTime(2018, 10, 1);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, filePrepDate).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            var progType = 50;
            var aimType = 1;
            var learnStartDate = new DateTime(2018, 7, 1);
            var filePrepDate = new DateTime(2018, 10, 1);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, filePrepDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var progType = 2;
            var aimType = 0;
            var learnStartDate = new DateTime(2018, 7, 1);
            var filePrepDate = new DateTime(2018, 10, 1);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, filePrepDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var progType = 2;
            var aimType = 1;
            var learnStartDate = new DateTime(2018, 9, 1);
            var filePrepDate = new DateTime(2018, 10, 1);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, filePrepDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(2, 1, "2018-7-1", "2018-10-1")]
        [InlineData(20, 1, "2018-7-1", "2018-10-1")]
        public void Validate_Error(int progType, int aimType, DateTime learnStartDate, DateTime filePrepDate)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = 999999999
                },
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = progType,
                        AimType = aimType,
                        LearnStartDate = learnStartDate
                    },
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);
            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(filePrepDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(2, 0, "2018-7-1", "2018-10-1")]
        [InlineData(2, 1, "2018-9-1", "2018-10-1")]
        public void Validate_NoError(int progType, int aimType, DateTime learnStartDate, DateTime filePrepDate)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = 999999999
                },
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = progType,
                        AimType = aimType,
                        LearnStartDate = learnStartDate
                    },
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);
            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(filePrepDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 8, 1));

            validationErrorHandlerMock.Verify();
        }

        private EmpId_13Rule NewRule(
            IDD07 dd07 = null,
            IFileDataService fileDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpId_13Rule(dd07, fileDataService, validationErrorHandler);
        }
    }
}
