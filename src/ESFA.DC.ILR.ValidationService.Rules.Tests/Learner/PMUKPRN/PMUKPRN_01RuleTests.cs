using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.ExternalData.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PMUKPRN;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PMUKPRN
{
    public class PMUKPRN_01RuleTests
    {
        [Fact]
        public void NullConditionMet_True()
        {
            NewRule().NullConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void NullConditionMet_False()
        {
            NewRule().NullConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LookupConditionMet_True()
        {
            NewRule().LookupConditionMet(false).Should().BeTrue();
        }

        [Fact]
        public void LookupConditionMet_False()
        {
            NewRule().LookupConditionMet(true).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PMUKPRNNullable = 1,
            };

            var organisationReferenceDataServiceMock = new Mock<IOrganisationReferenceDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            organisationReferenceDataServiceMock.Setup(ord => ord.UkprnExists(1)).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PMUKPRN_01", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(organisationReferenceDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner();

            NewRule().Validate(learner);
        }

        private PMUKPRN_01Rule NewRule(IOrganisationReferenceDataService organisationReferenceDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PMUKPRN_01Rule(organisationReferenceDataService, validationErrorHandler);
        }
    }
}
