using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class IQueryableExtensionsTests
    {
        [Fact]
        public async Task ToCaseInsensitiveDictionaryAsync_CaseInsensitive()
        {
            IQueryable<string> queryable = new List<string>() { "abc", "def" }.AsMockDbSet();

            var dictionary = await queryable.ToCaseInsensitiveAsyncDictionary(k => k, v => v, CancellationToken.None);

            dictionary["ABC"].Should().Be("abc");
        }

        [Fact]
        public void ToCaseInsensitiveDictionaryAsync_Null()
        {
            IQueryable<string> queryable = null;

            Func<Task> func = async () => await queryable.ToCaseInsensitiveAsyncDictionary(k => k, v => v, CancellationToken.None);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}
