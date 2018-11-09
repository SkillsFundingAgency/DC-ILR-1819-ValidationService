﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class ValidationOutputServiceTests
    {
        [Fact]
        public void SeverityToString_Warning()
        {
            NewService().SeverityToString(Severity.Warning).Should().Be("W");
        }

        [Fact]
        public void SeverityToString_Error()
        {
            NewService().SeverityToString(Severity.Error).Should().Be("E");
        }

        [Fact]
        public void SeverityToString_Fail()
        {
            NewService().SeverityToString(Severity.Fail).Should().Be("F");
        }

        [Fact]
        public void SeverityToString_Null()
        {
            NewService().SeverityToString(null).Should().BeNull();
        }

        [Fact]
        public void BuildInvalidLearnRefNumbers()
        {
            var validationErrors = new List<ValidationError>()
            {
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "a", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "a", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "b", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "b", Severity = "W" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "c", Severity = "W" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "c", Severity = "W" },
            };
            
            NewService().BuildInvalidLearnRefNumbers(validationErrors).Should().BeEquivalentTo("a", "b");
        }

        [Fact]
        public void BuildValidLearnRefNumbers()
        {
            var invalidLearnRefNumbers = new List<string>() { "a", "b" };

            var validationErrors = new List<ValidationError>()
            {
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "XYZ" },
            };

            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner() { LearnRefNumber = "a" },
                    new TestLearner() { LearnRefNumber = "b" },
                    new TestLearner() { LearnRefNumber = "c" },
                    new TestLearner() { LearnRefNumber = "d" },
                    new TestLearner() { LearnRefNumber = "e" },
                }
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            NewService(messageCache: messageCacheMock.Object).BuildValidLearnRefNumbers(invalidLearnRefNumbers, validationErrors).Should().BeEquivalentTo("c", "d", "e");
        }

        [Fact]
        public void BuildValidLearnRefNumbers_No_InvalidLearners()
        {
            var invalidLearnRefNumbers = new List<string>();

            var validationErrors = new List<ValidationError>()
            {
                new ValidationError() { RuleName = "HEADER", LearnerReferenceNumber = string.Empty},
            };

            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner() { LearnRefNumber = "a" },
                    new TestLearner() { LearnRefNumber = "b" },
                }
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            NewService(messageCache: messageCacheMock.Object).BuildValidLearnRefNumbers(invalidLearnRefNumbers, validationErrors).Should().HaveCount(2);
        }


        [Fact]
        public async Task SaveAsync()
        {
            var serializedValidLearners = "Serialized Valid Learners";
            var serializedInvalidLearners = "Serialized Invalid Learners";
            var serializedValidationErrors = "Serialized Validation Errors";
            var serializedValidationErrorMessageLookups = "Serialized Validation Error Message Lookups";

            var validLearnRefNumbersKey = "Valid Learn Ref Numbers Key";
            var invalidLearnRefNumbersKey = "Invalid Learn Ref Numbers Key";
            var validationErrorsKey = "Validation Errors Key";
            var validationErrorMessageLookupsKey = "Validation Error Message Lookups Key";

            IEnumerable<string> validLearnerRefNumbers = new List<string>() { "a", "b", "c" };
            IEnumerable<string> invalidLearnerRefNumbers = new List<string>() { "d", "e", "f" };
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>() { new ValidationError(), new ValidationError(), new ValidationError() };
            IEnumerable<ValidationErrorMessageLookup> validationErrorMessageLookups = new List<ValidationErrorMessageLookup> { new ValidationErrorMessageLookup(), new ValidationErrorMessageLookup(), new ValidationErrorMessageLookup() };

            var serializationServiceMock = new Mock<IJsonSerializationService>();
            var preValidationContextMock = new Mock<IPreValidationContext>();
            var keyValuePersistenceServiceMock = new Mock<IKeyValuePersistenceService>();

            serializationServiceMock.Setup(s => s.Serialize(validLearnerRefNumbers)).Returns(serializedValidLearners);
            serializationServiceMock.Setup(s => s.Serialize(invalidLearnerRefNumbers)).Returns(serializedInvalidLearners);
            serializationServiceMock.Setup(s => s.Serialize(validationErrors)).Returns(serializedValidationErrors);
            serializationServiceMock.Setup(s => s.Serialize(validationErrorMessageLookups)).Returns(serializedValidationErrorMessageLookups);

            preValidationContextMock.SetupGet(c => c.ValidLearnRefNumbersKey).Returns(validLearnRefNumbersKey);
            preValidationContextMock.SetupGet(c => c.InvalidLearnRefNumbersKey).Returns(invalidLearnRefNumbersKey);
            preValidationContextMock.SetupGet(c => c.ValidationErrorsKey).Returns(validationErrorsKey);
            preValidationContextMock.SetupGet(c => c.ValidationErrorMessageLookupKey).Returns(validationErrorMessageLookupsKey);

            keyValuePersistenceServiceMock.Setup(ps => ps.SaveAsync(validLearnRefNumbersKey, serializedValidLearners, default(CancellationToken))).Returns(Task.CompletedTask).Verifiable();
            keyValuePersistenceServiceMock.Setup(ps => ps.SaveAsync(invalidLearnRefNumbersKey, serializedInvalidLearners, default(CancellationToken))).Returns(Task.CompletedTask).Verifiable();
            keyValuePersistenceServiceMock.Setup(ps => ps.SaveAsync(validationErrorsKey, serializedValidationErrors, default(CancellationToken))).Returns(Task.CompletedTask).Verifiable();
            keyValuePersistenceServiceMock.Setup(ps => ps.SaveAsync(validationErrorMessageLookupsKey, serializedValidationErrorMessageLookups, default(CancellationToken))).Returns(Task.CompletedTask).Verifiable();

            var service = NewService(
                keyValuePersistenceService: keyValuePersistenceServiceMock.Object,
                preValidationContext: preValidationContextMock.Object,
                jsonSerializationService: serializationServiceMock.Object);

            await service.SaveAsync(validLearnerRefNumbers, invalidLearnerRefNumbers, validationErrors, validationErrorMessageLookups, CancellationToken.None);

            keyValuePersistenceServiceMock.VerifyAll();
        }

        private ValidationOutputService NewService(
            IValidationErrorCache<IValidationError> validationErrorCache = null,
            ICache<IMessage> messageCache = null,
            IKeyValuePersistenceService keyValuePersistenceService = null,
            IPreValidationContext preValidationContext = null,
            IJsonSerializationService jsonSerializationService = null,
            IValidationErrorsDataService validationErrorsDataService = null
            )
        {
            return new ValidationOutputService(
                validationErrorCache,
                messageCache,
                keyValuePersistenceService,
                preValidationContext,
                jsonSerializationService,
                validationErrorsDataService,
                new Mock<ILogger>().Object);
        }
    }
}
