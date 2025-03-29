
using Interface.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Repositories;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit_test;

namespace XUnitTest
{
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Logging.Abstractions;

    public class UnitOfWorkFixture : IDisposable
    {
        public IUnitOfWork UnitOfWork { get; private set; }
        public WebDemoDbContext Context { get; private set; }

        public UnitOfWorkFixture()
        {
            var options = TestHelper.CreateInMemoryDbContextOptions("TestDatabase");
            Context = new WebDemoDbContext(options);

            // Use the static instance of NullLogger
            var _logger = NullLogger<UnitOfWork>.Instance;

            // Remove transaction handling for in-memory provider
            UnitOfWork = new UnitOfWork(Context, _logger);
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
            Context.Dispose();
        }
    }
}