using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class TuyenKHRepository : LinqRepository<HDNHD.Models.DataContexts.Tuyenkhachhang>, ITuyenKHRepository
    {
        public TuyenKHRepository(DataContext context) : base(context) { }

        public IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> GetByNhanVienID(int nhanVienID)
        {
            HDNHDDataContext dc = (HDNHDDataContext)context;
            return GetAll(m => m.IsDelete == false).Join(
                dc.Tuyentheonhanviens.Where(m => m.NhanVienID == nhanVienID), 
                tuyenKH => tuyenKH.TuyenKHID,
                tuyenTheoNV => tuyenTheoNV.TuyenKHID, 
                (tuyenKH, tuyenTheoNV) => tuyenKH);
        }
    }
}