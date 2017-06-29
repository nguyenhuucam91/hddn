using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Repositories
{
    public class LinqUnitOfWork : IUnitOfWork
    {
        private IDictionary<Type, object> repositories;
        protected DataContext context;

        public LinqUnitOfWork(DataContext context)
        {
            this.context = context;
            repositories = new Dictionary<Type, object>();
        }

        public DataContext GetDataContext()
        {
            return this.context;
        }

        /// <typeparam name="TRepository">subclass of LinqRepository (NOTE: not an interface)</typeparam>
        /// <returns>an instance of TRepository : LinqRepository</returns>
        public TRepository Repository<TRepository>() where TRepository : IRepository
        {
            Type type = typeof(TRepository);

            if (!repositories.ContainsKey(type))
            {
                repositories[type] = (TRepository)Activator.CreateInstance(type, new object[] { context });
            }

            return (TRepository)repositories[type];
        }

        public IRepository<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            Type type = typeof(LinqRepository<TEntity>);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = (IRepository<TEntity>)Activator.CreateInstance(type, new object[] { context });
            }

            return (IRepository<TEntity>)repositories[type];
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

        public void BeginTransaction()
        {
            if (context.Connection.State != System.Data.ConnectionState.Open)
            {
                context.Connection.Open();
            }
            context.Transaction = context.Connection.BeginTransaction();
        }

        public void Commit()
        {
            context.Transaction.Commit();
            context.Connection.Close();
        }

        public void RollBack()
        {
            context.Transaction.Rollback();
            context.Connection.Close();
        }

        public int ExecuteCommand(String command, params object[] parameters)
        {
            return context.ExecuteCommand(command, parameters);
        }

        #region disposable
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    repositories.Clear();
                    repositories = null;
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