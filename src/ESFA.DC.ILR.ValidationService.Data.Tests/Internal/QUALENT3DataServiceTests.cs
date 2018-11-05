using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class QUALENT3DataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMock.Setup(p => p.Contains(LookupTimeRestrictedKey.QualEnt3, TypeOfQualEnt3.AwardAtLevel3)).Returns(true);

            NewService(provideLookupDetails: provideLookupDetailsMock.Object).Exists(TypeOfQualEnt3.AwardAtLevel3).Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMock.Setup(p => p.Contains(LookupTimeRestrictedKey.QualEnt3, "C1")).Returns(false);

            NewService(provideLookupDetails: provideLookupDetailsMock.Object).Exists("C1").Should().BeFalse();
        }

        [Fact]
        public void IsLearnStartDateBeforeValidTo_False()
        {
            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMock.Setup(p => p.IsCurrent(LookupTimeRestrictedKey.QualEnt3, TypeOfQualEnt3.CambridgePreUDiploma31072013, new DateTime(2019, 01, 01))).Returns(false);

            NewService(provideLookupDetails: provideLookupDetailsMock.Object).IsLearnStartDateBeforeValidTo(TypeOfQualEnt3.CambridgePreUDiploma31072013, new DateTime(2019, 01, 01)).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2013/01/01")]
        [InlineData(TypeOfQualEnt3.AwardAtLevel3, "2018/10/01")]
        public void IsLearnStartDateBeforeValidTo_True(string qualent3, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMock.Setup(p => p.IsCurrent(LookupTimeRestrictedKey.QualEnt3, qualent3, learnStartDate)).Returns(true);

            NewService(provideLookupDetails: provideLookupDetailsMock.Object).IsLearnStartDateBeforeValidTo(qualent3, learnStartDate).Should().BeTrue();
        }

        private QUALENT3DataService NewService(IProvideLookupDetails provideLookupDetails = null)
        {
            return new QUALENT3DataService(provideLookupDetails: provideLookupDetails);
        }
    }
}
