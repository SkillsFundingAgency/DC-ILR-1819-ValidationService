using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDCat
{
    public class LLDDCat_Rule02Tests : AbstractRuleTests<LLDDCat_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDCat_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var llddCat = 1;
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2017, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 07, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            var llddCatDataServiceMock = new Mock<IProvideLookupDetails>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 01, 01));
            llddCatDataServiceMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.LLDDCat, llddCat)).Returns(true);
            llddCatDataServiceMock.Setup(ds => ds.IsCurrent(TypeOfLimitedLifeLookup.LLDDCat, llddCat, dd06Mock.Object.Derive(learningDeliveries))).Returns(false);

            NewRule(dd06Mock.Object, llddCatDataServiceMock.Object).ConditionMet(llddCat, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var llddCat = 1;
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2017, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 07, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            var llddCatDataServiceMock = new Mock<IProvideLookupDetails>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 01, 01));
            llddCatDataServiceMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.LLDDCat, llddCat)).Returns(true);
            llddCatDataServiceMock.Setup(ds => ds.IsCurrent(TypeOfLimitedLifeLookup.LLDDCat, llddCat, dd06Mock.Object.Derive(learningDeliveries))).Returns(true);

            NewRule(dd06Mock.Object, llddCatDataServiceMock.Object).ConditionMet(llddCat, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var llddCat = 1;
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2017, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 07, 01)
                }
            };

            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        PrimaryLLDDNullable = 1,
                        LLDDCat = llddCat
                    }
                },
                LearningDeliveries = learningDeliveries
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            var llddCatDataServiceMock = new Mock<IProvideLookupDetails>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 01, 01));
            llddCatDataServiceMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.LLDDCat, llddCat)).Returns(true);
            llddCatDataServiceMock.Setup(ds => ds.IsCurrent(TypeOfLimitedLifeLookup.LLDDCat, llddCat, dd06Mock.Object.Derive(learningDeliveries))).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd06Mock.Object, llddCatDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var llddCat = 1;
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2017, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 07, 01)
                }
            };

            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem
                    {
                        PrimaryLLDDNullable = 1,
                        LLDDCat = llddCat
                    }
                },
                LearningDeliveries = learningDeliveries
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            var llddCatDataServiceMock = new Mock<IProvideLookupDetails>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 01, 01));
            llddCatDataServiceMock.Setup(ds => ds.IsCurrent(TypeOfLimitedLifeLookup.LLDDCat, llddCat, dd06Mock.Object.Derive(learningDeliveries))).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd06Mock.Object, llddCatDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private LLDDCat_02Rule NewRule(
            IDerivedData_06Rule dd06 = null,
            IProvideLookupDetails provideLookupDetails = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDCat_02Rule(dd06, provideLookupDetails, validationErrorHandler);
        }
    }
}