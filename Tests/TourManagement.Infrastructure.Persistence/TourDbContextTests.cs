using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Persistence;
using System;

namespace TourManagement.Infrastructure.Persistence.Tests
{
    public class TourDbContextTests
    {
        [Fact]
        public void DbContext_CanBeInstantiated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            // Act
            using var context = new TourDbContext(options);

            // Assert
            Assert.NotNull(context);
        }
    }
}
