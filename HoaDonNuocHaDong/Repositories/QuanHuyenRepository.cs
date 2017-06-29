using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class QuanHuyenRepository : LinqRepository<HDNHD.Models.DataContexts.Quanhuyen>, IQuanHuyenRepository
    {
        public QuanHuyenRepository(DataContext context) : base(context) { }
    }
}