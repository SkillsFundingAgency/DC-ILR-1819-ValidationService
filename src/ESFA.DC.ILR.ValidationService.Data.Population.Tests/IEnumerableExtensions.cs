using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Population.Tests.DbAsync;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public static class IEnumerableExtensions
    {
        public static System.Data.Entity.DbSet<T> AsMockDbSet<T>(this IEnumerable<T> sourceList)
            where T : class
        {
            var mockData = sourceList.AsQueryable();

            var mockSet = new Mock<System.Data.Entity.DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(mockData.Provider));
            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(mockData.GetEnumerator()));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            return mockSet.Object;
        }

        public static Microsoft.EntityFrameworkCore.DbSet<T> AsEFCoreMockDbSet<T>(this IEnumerable<T> sourceList)
            where T : class
        {
            var mockData = sourceList.AsQueryable();

            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(mockData.Provider));
            mockSet.As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(mockData.GetEnumerator()));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            return mockSet.Object;
        }
    }
}
