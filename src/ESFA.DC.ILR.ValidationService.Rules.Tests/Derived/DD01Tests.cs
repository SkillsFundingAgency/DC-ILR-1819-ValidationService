using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DD01Tests
    {
        public DD01 NewDD()
        {
            return new DD01();
        }

        [Fact]
        public void CalculateChecksum()
        {
            NewDD().CalculateCheckSum(1000000043).Should().Be(7);
        }

        [Fact]
        public void Derive_UlnNull()
        {
            NewDD().Derive(null).Should().Be("N");
        }

        [Fact]
        public void Derive_UlnNotTenCharacters()
        {
            NewDD().Derive(100000004).Should().Be("N");
        }

        [Fact]
        public void Derive_RemainderZero()
        {
            NewDD().Derive(1000000063).Should().Be("N");
        }

        [Fact]
        public void Derive_Correct()
        {
            NewDD().Derive(1000000043).Should().Be("3");
        }

        [Fact]
        public void Derive_TemporaryULN()
        {
            NewDD().Derive(9999999999).Should().Be("Y");
        }
    }
}
