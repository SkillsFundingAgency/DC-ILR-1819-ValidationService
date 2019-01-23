using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_31RuleTests
    {
        [Fact]
        public void IsAdultSkillsFundedEnglishOrMathsAim_True()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveryMock = new Mock<ILearningDelivery>();
            learningDeliveryMock.SetupGet(ld => ld.LearnAimRef).Returns(learnAimRef);
            learningDeliveryMock.SetupGet(ld => ld.LearnStartDate).Returns(learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), learnAimRef, learnStartDate)).Returns(true);

            NewRule(larsDataServiceMock.Object).IsAdultSkillsFundedEnglishOrMathsAim(learningDeliveryMock.Object);
        }

        [Fact]
        public void IsAdultSkillsFundedEnglishOrMathsAim_False()
        {
            var fundModel = 35;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveryMock = new Mock<ILearningDelivery>();
            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(fundModel);
            learningDeliveryMock.SetupGet(ld => ld.LearnAimRef).Returns(learnAimRef);
            learningDeliveryMock.SetupGet(ld => ld.LearnStartDate).Returns(learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), learnAimRef, learnStartDate)).Returns(true);

            NewRule(larsDataServiceMock.Object).IsAdultSkillsFundedEnglishOrMathsAim(learningDeliveryMock.Object);
        }

        [Fact]
        public void IsAdultSkilsFundedEnglishOrMathsAim_False()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveryMock = new Mock<ILearningDelivery>();
            learningDeliveryMock.SetupGet(ld => ld.LearnAimRef).Returns(learnAimRef);
            learningDeliveryMock.SetupGet(ld => ld.LearnStartDate).Returns(learnStartDate);

            NewRule().IsAdultSkillsFundedEnglishOrMathsAim(learningDeliveryMock.Object);
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(36)]
        [InlineData(34)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void BasicSkillsConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), learnAimRef, learnStartDate)).Returns(true);

            NewRule(larsDataServiceMock.Object).BasicSkillsConditionMet(learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void BasicSkillsConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), learnAimRef, learnStartDate)).Returns(false);

            NewRule(larsDataServiceMock.Object).BasicSkillsConditionMet(learnAimRef, learnStartDate).Should().BeFalse();
        }

        private DerivedData_31Rule NewRule(ILARSDataService larsDataService = null)
        {
            return new DerivedData_31Rule(larsDataService);
        }
    }
}
