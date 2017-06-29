using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class HDNHDUnitOfWork : LinqUnitOfWork
    {
        public HDNHDUnitOfWork()
            : base(new HDNHDDataContext())
        {
        }
    }
}