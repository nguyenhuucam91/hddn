using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class AdminUnitOfWork : LinqUnitOfWork
    {
        public AdminUnitOfWork(AdminDataContext context) : base(context) { }
    }
}