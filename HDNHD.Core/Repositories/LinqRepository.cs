using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HDNHD.Core.Repositories
{
    public class LinqRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DataContext context;
        protected Table<TEntity> table;

        public LinqRepository(DataContext context)
        {
            this.context = context;
            this.table = context.GetTable<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = table;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return this.table;
        }

        public TEntity GetSingle(Func<TEntity, bool> expression)
        {
            return table.SingleOrDefault(expression);
        }

        public virtual TEntity GetByID(object id)
        {
            var itemParameter = Expression.Parameter(typeof(TEntity), "item");
            var whereExpression = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(Expression.Property(itemParameter, PrimaryKeyName), Expression.Constant(id)),
                new[] { itemParameter }
            );

            return table.Where(whereExpression).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            table.InsertOnSubmit(entity);
        }

        public virtual void InsertAll(IEnumerable<TEntity> entities)
        {
            table.InsertAllOnSubmit(entities);
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = GetByID(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            table.DeleteOnSubmit(entityToDelete);
        }

        public bool SubmitChanges()
        {
            try
            {
                context.SubmitChanges();
                return true;
            }
            catch (Exception e)
            {
                if (context.Transaction != null)
                    throw e;

                return false;
            }
        }

        #region Properties

        private string PrimaryKeyName
        {
            get { return TableMetadata.RowType.IdentityMembers[0].Name; }
        }

        private System.Data.Linq.Mapping.MetaTable TableMetadata
        {
            get { return context.Mapping.GetTable(typeof(TEntity)); }
        }
        #endregion

        #region Disposable
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}