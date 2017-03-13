using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HDNHD.Core.Repositories.Interfaces
{
    /// <summary>
    /// ref: http://stackoverflow.com/questions/1541152/c-sharp-generic-where-constraint-with-any-generic-type-definition
    /// </summary>
    public interface IRepository { }

    public interface IRepository<TEntity> : IRepository, IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        IQueryable<TEntity> GetAll();

        TEntity GetSingle(Func<TEntity, bool> expression);

        TEntity GetByID(object id);

        void Insert(TEntity entity);

        void InsertAll(IEnumerable<TEntity> entities);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        bool SubmitChanges();
    }
}
