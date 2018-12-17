using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R43RuleTests : AbstractRuleTests<R47Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R43");
        }

        [Fact]
        public void ValidationPasses()
        {
            var date = new DateTime(2018, 12, 17, 9, 0, 0);

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>
                {
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = date
                    },
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = DateTime.Now
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var date = new DateTime(2018, 12, 17, 9, 0, 0);

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testLearner = new TestLearner
            {
                LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>
                {
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = date
                    },
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = date
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()));
        }

        private R43Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R43Rule(validationErrorHandler);
        }
    }
}
