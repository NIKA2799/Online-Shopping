using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_test
{
    public static class TestHelper
    {
        public static DbContextOptions<WebDemoDbContext> CreateInMemoryDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<WebDemoDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        public static TRepository CreateRepository<TRepository>(WebDemoDbContext dbContext)
            where TRepository : class
        {

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
            return (TRepository)Activator.CreateInstance(typeof(TRepository), dbContext);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
    }
}
