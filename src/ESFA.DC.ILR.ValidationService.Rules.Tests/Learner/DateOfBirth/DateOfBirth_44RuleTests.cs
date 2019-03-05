using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
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
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<string>()))
                .Returns(20);

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            if (contractIsAlsoNull)
            {
                fcsDataServiceMock
                    .Setup(m => m.GetContractAllocationFor(It.IsAny<string>()))
                    .Returns((IFcsContractAllocation)null);
            }
            else
            {
                fcsDataServiceMock
                    .Setup(m => m.GetContractAllocationFor(It.IsAny<string>()))
                    .Returns(new FcsContractAllocation
                    {
                        EsfEligibilityRule = null
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

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, fcsDataServiceMock.Object).Validate(testLearner);
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

        [Theory]
        [InlineData(20, 18, 20)]
        [InlineData(null, 18, 25)]
        [InlineData(20, 17, null)]
        [InlineData(20, null, 20)]
        [InlineData(20, null, null)]
        [InlineData(null, null, null)]
        public void ValidatePasses_AgeInRangeOrNull(int? age, int? minAge, int? maxAge)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testConRefNum = "12345";

            var dd23Rule = new Mock<IDerivedData_23Rule>();
            dd23Rule
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<string>()))
                .Returns(age);

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(It.IsAny<string>()))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule
                    {
                        MinAge = minAge,
                        MaxAge = maxAge
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

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, fcsDataServiceMock.Object).Validate(testLearner);
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

        [Theory]
        [InlineData(30, 18, 25)]
        [InlineData(20, 21, 25)]
        [InlineData(20, 21, 19)]
        [InlineData(20, 21, null)]
        [InlineData(20, null, 19)]
        public void ValidateFails(int? age, int? minAge, int? maxAge)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testConRefNum = "12345";

            var dd23Rule = new Mock<IDerivedData_23Rule>();
            dd23Rule
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<ILearner>(), It.IsAny<string>()))
                .Returns(age);

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(It.IsAny<string>()))
                .Returns(new FcsContractAllocation
                        {
                            EsfEligibilityRule = new EsfEligibilityRule
                            {
                                MinAge = minAge,
                                MaxAge = maxAge
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

            NewRule(validationErrorHandlerMock.Object, dd23Rule.Object, fcsDataServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_44Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_23Rule derivedDataRule23 = null,
            IFCSDataService fcsDataService = null)
        {
            return new DateOfBirth_44Rule(fcsDataService, derivedDataRule23, validationErrorHandler);
        }
    }
}
