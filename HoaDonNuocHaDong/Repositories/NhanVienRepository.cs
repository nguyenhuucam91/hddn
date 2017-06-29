using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class NhanVienRepository : LinqRepository<HDNHD.Models.DataContexts.Nhanvien>, INhanVienRepository
    {
        public NhanVienRepository(DataContext context) : base(context) { }

        public IQueryable<HDNHD.Models.DataContexts.Nhanvien> GetByToID(int toID)
        {
            return GetAll(m => m.ToQuanHuyenID == toID && m.IsDelete == false);
        }
    }
}