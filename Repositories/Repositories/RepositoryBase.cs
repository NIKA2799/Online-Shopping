using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Interface;
using Interface.IRepositories;
namespace Repositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Dto;
    using Interface;
    using Interface.IRepositories;
    using Microsoft.EntityFrameworkCore;

    namespace Repositories.Repositories
    {
        public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
            where TEntity : class, IEntity
        {
            protected readonly WebDemoDbContext _dbContext;
            protected readonly DbSet<TEntity> _dbset;

            // Compiled query: Exists by Id (per closed generic type TEntity)
            private static readonly Func<WebDemoDbContext, int, bool> _existsByIdCompiled =
                EF.CompileQuery((WebDemoDbContext ctx, int id) =>
                    ctx.Set<TEntity>().AsNoTracking().Any(e => e.Id == id));

            public RepositoryBase(WebDemoDbContext dbContext)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _dbset = dbContext.Set<TEntity>();
            }

            // --- Reads ---
            public TEntity GetById(params object[] id)
                => _dbset.Find(id) ?? throw new KeyNotFoundException($"Record with key {string.Join(",", id)} not found");

            public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> predicate)
                => _dbset.AsNoTracking().Where(predicate);

            public IQueryable<TEntity> FindAll()
                => _dbset.AsNoTracking();

            public IEnumerable<TEntity> GetAll()
                => _dbset.AsNoTracking().ToList();

            public TEntity? GetById(int id)
                => _dbset.Find(id);

            // --- Create/Update/Delete (CUD) ---
            public void Insert(TEntity entity)
            {
                if (entity is null) throw new ArgumentNullException(nameof(entity));
                _dbset.Add(entity);
            }

            public void Update(TEntity entity)
            {
                if (entity is null) throw new ArgumentNullException(nameof(entity));
                _dbset.Update(entity);
            }

            public void Delete(object id)
            {
                if (id is null) throw new ArgumentNullException(nameof(id));

                // ავირიდოთ ზედმეტი SELECT: ვაშლი სტაბით (ვგულისხმობ რომ IEntity.Id არის int)
                var stub = Activator.CreateInstance<TEntity>();
                stub.Id = Convert.ToInt32(id);
                _dbset.Attach(stub);
                _dbset.Remove(stub);
            }

            public void Delete(TEntity entity)
            {
                if (entity is null) throw new ArgumentNullException(nameof(entity));
                _dbset.Remove(entity);
            }

            public void DeleteRange(IEnumerable<TEntity> entities)
            {
                if (entities is null) throw new ArgumentNullException(nameof(entities));
                _dbset.RemoveRange(entities);
            }

            public void UpdateRange(IEnumerable<TEntity> entities)
            {
                if (entities is null) throw new ArgumentNullException(nameof(entities));
                _dbset.UpdateRange(entities);
            }

            public void InsertRange(IEnumerable<TEntity> entities)
            {
                if (entities is null) throw new ArgumentNullException(nameof(entities));
                _dbset.AddRange(entities);
            }

            public bool Exists(int id)
            {
                return _dbset.Any(x => x.Id == id);
            }
        }
    }
}