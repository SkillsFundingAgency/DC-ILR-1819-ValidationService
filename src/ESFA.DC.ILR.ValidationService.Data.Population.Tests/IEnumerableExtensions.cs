using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public static class IEnumerableExtensions
    {
        public static DbSet<T> AsMockDbSet<T>(this IEnumerable<T> sourceList)
            where T : class
        {
            var mockData = sourceList.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            return mockSet.Object;
        }
    }
}
