using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Utility;
using FluentAssertions;
using System;
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
                    LearnStartDate = new DateTime(2015, 1, 1)
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2015, 10, 10)
                },
            };

            NewDD().Derive(learningDeliveries).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void DeriveFor_NullLearningDelivery()
        {
            Action action = () => NewDD().Derive(null);

            action.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Derive using empty collection thows invalid operation exception.
        /// i don't think it should be allowed to do this... so, what's the policy?
        /// </summary>
        [Fact]
        public void DeriveUsingEmptyCollectionThrowsInvalidOperationException()
        {
            // arrange
            var rule = NewDD();

            // act / assert
            Assert.Throws<InvalidOperationException>(() => rule.Derive(Collection.Empty<ILearningDelivery>()));
        }

        private DD06 NewDD()
        {
            return new DD06();
        }
    }
}
