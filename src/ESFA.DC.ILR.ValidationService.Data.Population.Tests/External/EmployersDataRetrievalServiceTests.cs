using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ReferenceData.Employers.Model;
using ESFA.DC.ReferenceData.Employers.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class EmployersDataRetrievalServiceTests
    {
        [Fact]
        public void GetUniqueEmployerIdsFromMessage_NullMessage()
        {
            NewService().GetUniqueEmployerIdsFromFile(null).Should().BeEmpty();
        }

        [Fact]
        public void GetUniqueEmployerIdsFromMessage_NullLearners()
        {
            NewService().GetUniqueEmployerIdsFromFile(new TestMessage()).Should().BeEmpty();
        }

        [Fact]
        public void GetUniqueEmployerIdsFromMessage_WorkPlaceEmpIds()
        {
            var empIdOne = 1;
            var empIdTwo = 2;
            var empIdThree = 3;

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<ILearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = new List<ILearningDeliveryWorkPlacement>()
                                {
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdOne
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdTwo
                                    },
                                }
                            },
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = new List<ILearningDeliveryWorkPlacement>()
                                {
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdTwo
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdThree
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = null,
                                    }
                                }
                            },
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = null
                            }
                        }
                    }
                }
            };

            var actual = NewService().GetUniqueEmployerIdsFromFile(message).ToList();

            actual.Should().HaveCount(3);
            actual.Should().Contain(new List<int>() { empIdOne, empIdTwo, empIdThree });
        }

        [Fact]
        public void GetUniqueEmployerIdsFromMessage_LearnerEmpIds()
        {
            var empIdOne = 1;
            var empIdTwo = 2;
            var empIdThree = 3;
            var empIdWorkPlacement = 4;
            var empIdLearnerEmploymentStatus = 5;

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new List<ILearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = new List<ILearningDeliveryWorkPlacement>()
                                {
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdOne
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdWorkPlacement
                                    },
                                }
                            },
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = new List<ILearningDeliveryWorkPlacement>()
                                {
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdTwo
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = empIdThree
                                    },
                                    new TestLearningDeliveryWorkPlacement()
                                    {
                                        WorkPlaceEmpIdNullable = null,
                                    }
                                }
                            },
                            new TestLearningDelivery()
                            {
                                LearningDeliveryWorkPlacements = null
                            }
                        },
                        LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
                        {
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdLearnerEmploymentStatus
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdTwo
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
                        {
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdTwo
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdThree
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = null
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearnerEmploymentStatuses = null
                    }
                }
            };

            var actual = NewService().GetUniqueEmployerIdsFromFile(message).ToList();

            actual.Should().HaveCount(5);
            actual.Should().Contain(new List<int>() { empIdOne, empIdTwo, empIdThree, empIdLearnerEmploymentStatus, empIdWorkPlacement });
        }

        [Fact]
        public void GetUniqueEmployerIdsFromMessage_EmpIds()
        {
            var empIdOne = 1;
            var empIdTwo = 2;
            var empIdThree = 3;

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    new TestLearner()
                    {
                        LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
                        {
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdOne
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdTwo
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
                        {
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdTwo
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = empIdThree
                            },
                            new TestLearnerEmploymentStatus()
                            {
                                EmpIdNullable = null
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearnerEmploymentStatuses = null
                    }
                }
            };

            var actual = NewService().GetUniqueEmployerIdsFromFile(message).ToList();

            actual.Should().HaveCount(3);
            actual.Should().Contain(new List<int>() { empIdOne, empIdTwo, empIdThree });
        }

        [Fact]
        public async Task GetMatchingEmployerIds()
        {
            var employers = new List<Employer>()
            {
                new Employer() { Urn = 1 },
                new Employer() { Urn = 2 },
                new Employer() { Urn = 3 },
            }.AsQueryable();

            var employerIdsInMessage = new List<int>() { 2, 3, 4 };

            var employersContextMock = new Mock<IEmployersContext>();

            employersContextMock.SetupGet(c => c.Employers).Returns(employers);

            var actual = await NewService(employersContextMock.Object).GetMatchingEmployerIds(employerIdsInMessage, CancellationToken.None);

            actual.Should().HaveCount(2);
            actual.Should().Contain(new List<int>() { 2, 3 });
        }

        private EmployersDataRetrievalService NewService(IEmployersContext employersContext = null, ICache<IMessage> messageCache = null)
        {
            return new EmployersDataRetrievalService(employersContext, messageCache);
        }
    }
}
