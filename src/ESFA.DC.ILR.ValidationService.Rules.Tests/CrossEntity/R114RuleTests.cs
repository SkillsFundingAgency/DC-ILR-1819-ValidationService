using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R114RuleTests : AbstractRuleTests<R114Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R114");
        }

        [Fact]
        public void Validate_Error()
        {
            var earliestOpenLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var adultSkillsFundedEnglishOrMathsAimOne = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    earliestOpenLearningDelivery,
                    adultSkillsFundedEnglishOrMathsAimOne,
                }
            };

            var dd32Mock = new Mock<IDerivedData_32Rule>();

            dd32Mock.Setup(dd => dd.IsOpenApprenticeshipFundedProgramme(earliestOpenLearningDelivery)).Returns(true);

            var dd31Mock = new Mock<IDerivedData_31Rule>();

            dd31Mock.Setup(dd => dd.IsAdultSkillsFundedEnglishOrMathsAim(adultSkillsFundedEnglishOrMathsAimOne)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd31Mock.Object, dd32Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var earliestOpenLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            var adultSkillsFundedEnglishOrMathsAimOne = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    earliestOpenLearningDelivery,
                    adultSkillsFundedEnglishOrMathsAimOne,
                }
            };

            var dd32Mock = new Mock<IDerivedData_32Rule>();

            dd32Mock.Setup(dd => dd.IsOpenApprenticeshipFundedProgramme(earliestOpenLearningDelivery)).Returns(true);

            var dd31Mock = new Mock<IDerivedData_31Rule>();

            dd31Mock.Setup(dd => dd.IsAdultSkillsFundedEnglishOrMathsAim(adultSkillsFundedEnglishOrMathsAimOne)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd31Mock.Object, dd32Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void DateConditionMet_True()
        {
            var adultSkillsEnglishOrMathsAim = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            var earliestOpenApprenticeshipFundedLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            NewRule().DateConditionMet(adultSkillsEnglishOrMathsAim, earliestOpenApprenticeshipFundedLearningDelivery).Should().BeTrue();
        }

        [Fact]
        public void DateConditionMet_False()
        {
            var adultSkillsEnglishOrMathsAim = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var earliestOpenApprenticeshipFundedLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            NewRule().DateConditionMet(adultSkillsEnglishOrMathsAim, earliestOpenApprenticeshipFundedLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void GetEarliestOpenApprenticeshipFundedLearningDelivery()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var laterLearningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            var notOpenApprenticeshipFundedLearningDelivery = new TestLearningDelivery();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    earliestLearningDelivery,
                    laterLearningDelivery,
                    notOpenApprenticeshipFundedLearningDelivery
                }
            };

            var dd32Mock = new Mock<IDerivedData_32Rule>();

            dd32Mock.Setup(dd => dd.IsOpenApprenticeshipFundedProgramme(earliestLearningDelivery)).Returns(true);
            dd32Mock.Setup(dd => dd.IsOpenApprenticeshipFundedProgramme(laterLearningDelivery)).Returns(true);
            dd32Mock.Setup(dd => dd.IsOpenApprenticeshipFundedProgramme(notOpenApprenticeshipFundedLearningDelivery)).Returns(false);

            NewRule(dd32: dd32Mock.Object).GetEarliestOpenApprenticeshipFundedLearningDelivery(learner).Should().Be(earliestLearningDelivery);
        }

        [Fact]
        public void GetEarliestOpenApprenticeshipFundedLearningDelivery_NullLearner()
        {
            NewRule().GetEarliestOpenApprenticeshipFundedLearningDelivery(null).Should().BeNull();
        }

        [Fact]
        public void GetEarliestOpenApprenticeshipFundedLearningDelivery_NullLearningDeliveries()
        {
            NewRule().GetEarliestOpenApprenticeshipFundedLearningDelivery(new TestLearner()).Should().BeNull();
        }

        [Fact]
        public void GetAdultSkillsEnglishOrMathsAims()
        {
            var adultSkillsFundedEnglishOrMathsAimOne = new TestLearningDelivery();
            var adultSkillsFundedEnglishOrMathsAimTwo = new TestLearningDelivery();
            var nonAdultSkillsFundedEnglishOrMathsAimTwo = new TestLearningDelivery();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    adultSkillsFundedEnglishOrMathsAimOne,
                    adultSkillsFundedEnglishOrMathsAimTwo,
                    nonAdultSkillsFundedEnglishOrMathsAimTwo,
                }
            };

            var dd31Mock = new Mock<IDerivedData_31Rule>();

            dd31Mock.Setup(dd => dd.IsAdultSkillsFundedEnglishOrMathsAim(adultSkillsFundedEnglishOrMathsAimOne)).Returns(true);
            dd31Mock.Setup(dd => dd.IsAdultSkillsFundedEnglishOrMathsAim(adultSkillsFundedEnglishOrMathsAimTwo)).Returns(true);
            dd31Mock.Setup(dd => dd.IsAdultSkillsFundedEnglishOrMathsAim(nonAdultSkillsFundedEnglishOrMathsAimTwo)).Returns(false);

            NewRule(dd31Mock.Object).GetAdultSkillsEnglishOrMathsAims(learner).Should().Contain(new List<ILearningDelivery>() { adultSkillsFundedEnglishOrMathsAimOne, adultSkillsFundedEnglishOrMathsAimTwo });
        }

        [Fact]
        public void GetAdultSkillsEnglishOrMathsAims_NullLearner()
        {
            NewRule().GetAdultSkillsEnglishOrMathsAims(null).Should().BeEmpty();
        }

        [Fact]
        public void GetAdultSkillsEnglishOrMathsAims_NullLearningDelivery()
        {
            NewRule().GetAdultSkillsEnglishOrMathsAims(new TestLearner()).Should().BeEmpty();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2017, 1, 1);
            var fundModel = 35;
            var learnActEndDate = new DateTime(2018, 1, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType));
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate));
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel));
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimType, learnStartDate, fundModel, learnActEndDate);
        }

        private R114Rule NewRule(IDerivedData_31Rule dd31 = null, IDerivedData_32Rule dd32 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R114Rule(dd31, dd32, validationErrorHandler);
        }
    }
}
