﻿using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AimType.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AimType
{
    public class AimType_01RuleTests : AbstractRuleTests<AimType_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AimType_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;

            var aimTypeInternalDataServiceMock = new Mock<IAimTypeDataService>();

            aimTypeInternalDataServiceMock.Setup(ds => ds.Exists(aimType)).Returns(false);

            NewRule(aimTypeInternalDataServiceMock.Object).ConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var aimType = 1;

            var aimTypeInternalDataServiceMock = new Mock<IAimTypeDataService>();

            aimTypeInternalDataServiceMock.Setup(ds => ds.Exists(aimType)).Returns(true);

            NewRule(aimTypeInternalDataServiceMock.Object).ConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType
                    }
                }
            };

            var aimTypeInternalDataServiceMock = new Mock<IAimTypeDataService>();

            aimTypeInternalDataServiceMock.Setup(ds => ds.Exists(aimType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(aimTypeInternalDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var aimType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType
                    }
                }
            };

            var aimTypeInternalDataServiceMock = new Mock<IAimTypeDataService>();

            aimTypeInternalDataServiceMock.Setup(ds => ds.Exists(aimType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(aimTypeInternalDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AimType_01Rule NewRule(IAimTypeDataService aimTypeInternalDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AimType_01Rule(aimTypeInternalDataService, validationErrorHandler);
        }
    }
}