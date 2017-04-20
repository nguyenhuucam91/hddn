using System;
using System.Linq;
using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class KhachHangRepository : LinqRepository<HDNHD.Models.DataContexts.Khachhang>, IKhachHangRepository
    {
        public KhachHangRepository(HDNHDDataContext context) : base(context) { }

        public IQueryable<KhachHangModel> GetAllKhachHangModel()
        {
            var items = GetAll(m => m.IsDelete == null || m.IsDelete == false)
                .OrderBy(m => m.TuyenKHID)
                .OrderBy(m => m.TTDoc)
                .Select(m => new KhachHangModel()
                {
                    KhachHang = m
                });

            return items;
        }

        public KhachHangDetailsModel GetKhachHangDetailsModel(int id)
        {
            var context = (HDNHDDataContext)this.context;

            return (from kh in context.Khachhangs
                    where kh.KhachhangID == id && (kh.IsDelete == null || kh.IsDelete == false)
                    join t in context.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                    join qh in context.Quanhuyens on kh.QuanhuyenID equals qh.QuanhuyenID
                    join px in context.Phuongxas on kh.PhuongxaID equals px.PhuongxaID
                    join _cdc in context.Cumdancus on kh.CumdancuID equals _cdc.CumdancuID into gcdc
                    from cdc in gcdc.DefaultIfEmpty()
                    join _to in context.Tuyenongs on kh.TuyenongkythuatID equals _to.TuyenongID into gto
                    from to in gto.DefaultIfEmpty()
                    select new KhachHangDetailsModel()
                    {
                        KhachHang = kh,
                        TuyenKH = t,
                        QuanHuyen = qh,
                        PhuongXa = px,
                        CumDanCu = cdc,
                        TuyenOng = to
                    }).FirstOrDefault();
        }
    }
}