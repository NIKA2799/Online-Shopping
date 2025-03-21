using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.IRepositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(params object[] id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression);
        IQueryable<TEntity> FindAll();
        void Update(TEntity entity);
        TEntity? GetById(int id);
        public bool Exists(int id);

        public void DeleteRange(IEnumerable<TEntity> entities);
        public void UpdateRange(IEnumerable<TEntity> entities);
        public void InsertRange(IEnumerable<TEntity> entities);
    }
}
