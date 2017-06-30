using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Data.Linq;
using System.Linq;
using System;

namespace HoaDonNuocHaDong.Repositories
{
    public class TuyenKHRepository : LinqRepository<HDNHD.Models.DataContexts.Tuyenkhachhang>, ITuyenKHRepository
    {
        private HDNHDDataContext dc;

        public TuyenKHRepository(DataContext context) : base(context)
        {
            dc = (HDNHDDataContext)context;
        }

        public override IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> GetAll()
        {
            return base.GetAll(m => m.IsDelete == null || m.IsDelete == false);
        }

        public IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> GetByNhanVienID(int nhanVienID)
        {
            return GetAll().Join(
                dc.Tuyentheonhanviens.Where(m => m.NhanVienID == nhanVienID),
                tuyenKH => tuyenKH.TuyenKHID,
                tuyenTheoNV => tuyenTheoNV.TuyenKHID,
                (tuyenKH, tuyenTheoNV) => tuyenKH);
        }

        public IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> GetByToID(int toID)
        {
            return from item in GetAll()
                   join ttnv in dc.Tuyentheonhanviens on item.TuyenKHID equals ttnv.TuyenKHID
                   join nv in dc.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                   where nv.ToQuanHuyenID == toID
                   select item;
        }
    }
}