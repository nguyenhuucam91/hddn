using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class DuCoRepository : LinqRepository<HDNHD.Models.DataContexts.DuCo>, IDuCoRepository
    {
        public DuCoRepository(DataContext context) : base(context) { }
    }
}