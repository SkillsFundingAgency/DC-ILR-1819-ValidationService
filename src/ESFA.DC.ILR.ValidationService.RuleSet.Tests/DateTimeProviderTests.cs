using System;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class DateTimeProviderTests
    {
        [Fact]
        public void UtcNow()
        {
            var dateTimeProvider = new RuleSet.DateTimeProvider();

            dateTimeProvider.UtcNow.Should().BeCloseTo(DateTime.UtcNow, 50);
        }
        
    }
}
