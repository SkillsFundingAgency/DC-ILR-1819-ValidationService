using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_44RuleTests : AbstractRuleTests<DateOfBirth_44Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_44");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ValidatePasses_NoMatchingContractEligibility(bool contractIsAlsoNull)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testConRefNum = "12345";

            var dd23Rule = new Mock<IDerivedData_23Rule>();
            dd23Rule
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(20);

            var externalCacheMock = new Mock<IExternalDataCache>();
            if (contractIsAlsoNull)
            {
                externalCacheMock
                    .Setup(m => m.FCSContractAllocations)
                    .Returns((IReadOnlyDictionary<string, IFcsContractAllocation>)null);
            }
            else
            {
                externalCacheMock
                    .Setup(m => m.FCSContractAllocations)
                    .Returns(new Dictionary<string, IFcsContractAllocation>
                    {
                        {
                            testConRefNum,
                            new FcsContractAllocation
                            {
                                EsfEligibilityRule = null
                            }
                        },
                        {
                            "9999999",
                            new FcsContractAllocation
                            {
                                EsfEligibilityRule = new EsfEligibilityRule
                                {
                                    MinAge = 18,
                                    MaxAge = 25
                                }
                            }
                        }
                    });
            }

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNum,
                        FundModel = 70
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, externalCacheMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_IrrelevantFundModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testConRefNum = "12345";

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNum,
                        FundModel = 35
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_AgeInRange()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testConRefNum = "12345";

            var dd23Rule = new Mock<IDerivedData_23Rule>();
            dd23Rule
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(20);

            var externalCacheMock = new Mock<IExternalDataCache>();
            externalCacheMock
                .Setup(m => m.FCSContractAllocations)
                .Returns(new Dictionary<string, IFcsContractAllocation>
                {
                    {
                        testConRefNum,
                        new FcsContractAllocation
                        {
                            EsfEligibilityRule = new EsfEligibilityRule
                            {
                                MinAge = 18,
                                MaxAge = 25
                            }
                        }
                    }
                });

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNum,
                        FundModel = 70
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, externalCacheMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(2014, 8, 31)
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testConRefNum = "12345";

            var dd23Rule = new Mock<IDerivedData_23Rule>();
            dd23Rule
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(30);

            var externalCacheMock = new Mock<IExternalDataCache>();
            externalCacheMock
                .Setup(m => m.FCSContractAllocations)
                .Returns(new Dictionary<string, IFcsContractAllocation>
                {
                    {
                        testConRefNum,
                        new FcsContractAllocation
                        {
                            EsfEligibilityRule = new EsfEligibilityRule
                            {
                                MinAge = 18,
                                MaxAge = 25
                            }
                        }
                    }
                });

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1988, 8, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNum,
                        FundModel = 70
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, externalCacheMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_44Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_23Rule derivedDataRule23 = null,
            IExternalDataCache externalDataCache = null)
        {
            return new DateOfBirth_44Rule(externalDataCache, derivedDataRule23, validationErrorHandler);
        }

        private void VerifyErrorHandlerMock(ValidationErrorHandlerMock errorHandlerMock, int times = 0)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Exactly(times));
        }
    }
}
