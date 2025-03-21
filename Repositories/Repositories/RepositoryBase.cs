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
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class, IEntity
    {
        protected readonly WebDemoDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbset;
        public RepositoryBase(WebDemoDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbset = dbContext.Set<TEntity>();
        }
        public TEntity GetById(params object[]id)=> _dbset.Find(id)?? throw new KeyNotFoundException($"Record with key {id} not found");
        public IQueryable<TEntity> FindByCondition(Expression <Func<TEntity, bool>> predicate) => _dbset.Where(predicate).AsNoTracking();
        public IQueryable<TEntity> FindAll() => _dbset;
        public void Insert(TEntity entity)
        {
            if (entity == null) throw new ArgumentException(nameof(entity));
            _dbset.Add(entity);
        }
        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbset.Update(entity);
        }
        public void Delete(object id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            Delete(GetById(id));
        }

        public void Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dbset.Remove(entity);
        }
        public bool Any(Func<TEntity, bool> predicate)
        {
            return _dbset.Any(predicate);
        }
        public bool Exists(int id)
        {
            return _dbset.Any(x => x.Id == id);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
        }
        public void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return _dbset.ToList();
        } 
        public TEntity? GetById(int id)
        {
            return _dbset.Find(id);
        }
    }
}