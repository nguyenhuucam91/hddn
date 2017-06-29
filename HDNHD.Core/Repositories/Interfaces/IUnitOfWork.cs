using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HDNHD.Core.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// returns an instance of custom extended from base repository
        /// eg: UserRepository : LinqRepository<User>
        /// </summary>
        TRepository Repository<TRepository>() where TRepository : IRepository;

        /// <summary>
        /// returns an instance of base repository
        /// eg: LinqRepository<TEntity>, EFRepository<TEntity>
        /// </summary>
        IRepository<TEntity> BaseRepository<TEntity>() where TEntity : class;

        bool SubmitChanges();

        void BeginTransaction();

        void Commit();

        void RollBack();

        int ExecuteCommand(String command, params object[] parameters);
    }
}
