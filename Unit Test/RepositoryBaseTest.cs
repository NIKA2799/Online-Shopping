
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using Repositories;
using Interface.IRepositories;


namespace Unit_test
{
    public abstract class RepositoryBaseTest<TEntity, TRepository> : IDisposable
      where TEntity : class, new()
      where TRepository : class
    {
        protected readonly WebDemoDbContext _dbContext;
        protected readonly TRepository _repository;
        protected readonly IUnitOfWork _unitOfWork;

        protected RepositoryBaseTest(string databaseName, Func<WebDemoDbContext, TRepository> repositoryFactory, IUnitOfWork unitOfWork)
        {
            DbContextOptions<WebDemoDbContext> options = TestHelper.CreateInMemoryDbContextOptions(databaseName);
            _dbContext = new WebDemoDbContext(options);
            _repository = repositoryFactory(_dbContext);
            _unitOfWork = unitOfWork;
        }

        public TRepository Repository => _repository;

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}