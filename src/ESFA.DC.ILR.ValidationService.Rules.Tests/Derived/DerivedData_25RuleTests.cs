using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_25RuleTests
    {
        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsNullForNullLearner()
        {
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = 5
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);

            var result = rule.GetLengthOfUnemployment(null, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsNullForNullLearningDeliveries()
        {
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = 5
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);
            var learner = new TestLearner
            {
                LearningDeliveries = null
            };

            var result = rule.GetLengthOfUnemployment(learner, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsNullForIrrelevantConRefNumber()
        {
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = 5
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2
                    }
                }
            };

            var result = rule.GetLengthOfUnemployment(learner, "99999");

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsNullForNoZESF0001Deliverable()
        {
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = 5
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = "FOO",
                        CompStatus = 2
                    }
                }
            };

            var result = rule.GetLengthOfUnemployment(learner, testConRefNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsNullForNoZESF0001DeliverableCompleted()
        {
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = 5
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = "ZESF0001",
                        CompStatus = 1
                    }
                }
            };

            var result = rule.GetLengthOfUnemployment(learner, testConRefNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsCorrectAge()
        {
            var expectedResult = 5;
            var serviceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            serviceMock
                .Setup(m => m.LearnerEmploymentStatusForDate(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<DateTime>()))
                .Returns(new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "LOU",
                            ESMCode = expectedResult
                        }
                    }
                });

            var rule = NewRule(serviceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2
                    }
                }
            };

            var result = rule.GetLengthOfUnemployment(learner, testConRefNumber);

            Assert.Equal(expectedResult, result);
        }

        public DerivedData_25Rule NewRule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null)
        {
            return new DerivedData_25Rule(learnerEmploymentStatusQueryService);
        }
    }
}
