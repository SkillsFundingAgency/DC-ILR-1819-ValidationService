using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DD06Tests
    {
        [Fact]
        public void Derive()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    LearnStartDateNullable = new DateTime(2015, 1, 1)
                },
                new TestLearningDelivery()
                {
                    LearnStartDateNullable = new DateTime(2015, 10, 10)
                },
            };

            var dd06 = new DD06();

            dd06.Derive(learningDeliveries).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void DeriveFor_NullLearningDelivery()
        {
            var dd06 = new DD06();

            dd06.Derive(null).Should().BeNull();
        }

        [Fact]
        public void Derive_ForOne_OneLearningDeliveryStartNull()
        {
            var learningDeliveries = new TestLearningDelivery[]
             {
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = null
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = new DateTime(2015, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = new DateTime(2015, 10, 10)
                    },
            };

            var dd06 = new DD06();
            dd06.Derive(learningDeliveries).Should().Be(new DateTime(2015, 1, 1));
        }
    }
}
