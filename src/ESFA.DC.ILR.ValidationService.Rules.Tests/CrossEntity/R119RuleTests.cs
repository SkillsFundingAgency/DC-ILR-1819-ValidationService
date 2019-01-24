using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R119RuleTests : AbstractRuleTests<R119Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R119");
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2019, 01, 31)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2019, 02, 01)).Should().BeTrue();
        }

        [Theory]
        [InlineData("PMR", 0, "2018-07-01", "2018-08-01")]
        [InlineData("TNP", 0, "2018-07-01", "2018-08-01")]
        [InlineData("TNP", 1, "2018-07-01", "2018-08-01")]
        [InlineData("TNP", 0, "2018-07-01", "2018-06-01")]
        public void AppFinRecodConditionMet_False(string aFinType, int aFinCode, string learnStartDateString, string aFinDateString)
        {
            int? aFinCodeOut;
            DateTime? aFinDateOut;
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            var appFinRecords = new TestAppFinRecord[]
                {
                    new TestAppFinRecord()
                    {
                         AFinType = aFinType,
                         AFinCode = aFinCode,
                         AFinDate = DateTime.Parse(aFinDateString)
                    },
                    null
                };

            NewRule().AppFinRecodConditionMet(appFinRecords, learnStartDate, out aFinCodeOut, out aFinDateOut).Should().BeFalse();
            aFinCodeOut.Should().BeNull();
            aFinDateOut.Should().BeNull();
        }

        [Fact]
        public void AppFinRecodConditionMet_True()
        {
            int? aFinCodeOut;
            DateTime? aFinDateOut;
            int aFinCodeExpected = 1;
            DateTime aFinDateExpected = new DateTime(2018, 07, 01);
            DateTime learnStartDate = new DateTime(2018, 11, 01);
            var appFinRecords = new TestAppFinRecord[]
                {
                    new TestAppFinRecord()
                    {
                         AFinType = "TNP",
                         AFinCode = aFinCodeExpected,
                         AFinDate = aFinDateExpected
                    },
                    null,
                    new TestAppFinRecord()
                    {
                         AFinType = "TNP",
                         AFinCode = 2,
                         AFinDate = new DateTime(2017, 01, 01)
                    },
                };

            NewRule().AppFinRecodConditionMet(appFinRecords, learnStartDate, out aFinCodeOut, out aFinDateOut).Should().BeTrue();
            aFinCodeOut.Should().Be(aFinCodeExpected);
            aFinDateOut.Should().Be(aFinDateExpected);
        }

        [Fact]
        public void ConditionMet_False()
        {
            int? aFinCodeOut;
            DateTime? aFinDateOut;
            DateTime learnStartDate = new DateTime(2018, 12, 31);
            var appFinRecords = new TestAppFinRecord[]
                {
                    new TestAppFinRecord()
                    {
                         AFinType = "PMR",
                         AFinCode = 0,
                         AFinDate = new DateTime(2019, 01, 01)
                    },
                    null
                };

            NewRule().ConditionMet(learnStartDate, appFinRecords, out aFinCodeOut, out aFinDateOut).Should().BeFalse();
            aFinCodeOut.Should().BeNull();
            aFinDateOut.Should().BeNull();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int? aFinCodeOut;
            int aFinCodeExpected = 1;
            DateTime? aFinDateOut;
            DateTime aFinDateExpected = new DateTime(2019, 02, 01);
            DateTime learnStartDate = new DateTime(2019, 02, 28);
            var appFinRecords = new TestAppFinRecord[]
                {
                    new TestAppFinRecord()
                    {
                         AFinType = "TNP",
                         AFinCode = aFinCodeExpected,
                         AFinDate = aFinDateExpected
                    },
                    null
                };

            NewRule().ConditionMet(learnStartDate, appFinRecords, out aFinCodeOut, out aFinDateOut).Should().BeTrue();
            aFinCodeOut.Should().Be(aFinCodeExpected);
            aFinDateOut.Should().Be(aFinDateExpected);
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A1234",
                        LearnStartDate = new DateTime(2019, 02, 28),
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "TNP",
                                 AFinCode = 2,
                                 AFinDate = new DateTime(2019, 02, 01)
                            },
                            null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A1234",
                        LearnStartDate = new DateTime(2019, 01, 31),
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "PMR",
                                 AFinCode = 0,
                                 AFinDate = new DateTime(2019, 02, 01)
                            },
                            null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnStartDate", "01/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinType", "PNR")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinDate", "01/11/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                new DateTime(2018, 07, 01),
                "PNR",
                1,
                new DateTime(2018, 11, 01));
            validationErrorHandlerMock.Verify();
        }

        public R119Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R119Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
