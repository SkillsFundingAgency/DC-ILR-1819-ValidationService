﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Ethnicity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Ethnicity
{
    public class Ethnicity_01RuleTests : AbstractRuleTests<Ethnicity_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Ethnicity_01");
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                Ethnicity = -99
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(LookupSimpleKey.Ethnicity, -99)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                Ethnicity = TypeOfEthnicity.English_Welsh_Scottish_Northern_Irish_British
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(LookupSimpleKey.Ethnicity, TypeOfEthnicity.English_Welsh_Scottish_Northern_Irish_British)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        public Ethnicity_01Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null)
        {
            return new Ethnicity_01Rule(validationErrorHandler: validationErrorHandler, provideLookupDetails: provideLookupDetails);
        }
    }
}
