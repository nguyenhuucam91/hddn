using HDNHD.Core.Controllers;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class HoaDonController : BaseController
    {
        private IHoaDonRepository hoaDonRepository;
        private IToRepository toRepository;

        public HoaDonController()
        {
            hoaDonRepository = uow.Repository<HoaDonRepository>();
            toRepository = uow.Repository<ToRepository>();
        }

        /// <summary>
        /// load hiển thị dữ liệu hóa đơn
        /// </summary>
        public ActionResult Index(HoaDonFilterModel filter, HDNHD.Core.Models.Pager pager, String action)
        {
            // default values
            if (filter.Mode == null) // not in filter
            {
                if (filter.From == null && filter.To == null)
                    filter.From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                // set selected to, quan huyen = nhanVien's to, quan huyen
                if (nhanVien != null)
                {
                    filter.NhanVienID = nhanVien.NhanvienID;
                    filter.ToID = nhanVien.ToQuanHuyenID;

                    var to = toRepository.GetByID(nhanVien.ToQuanHuyenID ?? 0);
                    if (to != null)
                    {
                        filter.QuanHuyenID = to.QuanHuyenID;
                    }
                }
            }

            // query items
            var items = hoaDonRepository.GetAllModel();
            filter.ApplyFilter(ref items);

            // apply actions
            if (action == "DanhDauTatCa")
            {
                //uow.BeginTransaction();
                
                //try
                //{
                //    String sql = "UPDATE [HoaDonHaDong].[dbo].[Hoadonnuoc] SET Trangthaithu = 1, NgayNopTien = CAST(GETDATE() AS DATE) from Hoadonnuoc join [HoaDonHaDong].[dbo].Khachhang on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID  WHERE Hoadonnuoc.ThangHoaDon =" + now.Month + " and Hoadonnuoc.NamHoaDon = " + now.Year + "  and Khachhang.QuanhuyenID =" + chinhanh + ";";
                    
                //    String sql2 = "UPDATE [HoaDonHaDong].[dbo].SoTienNopTheoThang SET SoTienDaThu = SoTienPhaiNop from SoTienNopTheoThang join [HoaDonHaDong].[dbo].Hoadonnuoc on SoTienNopTheoThang.HoaDonNuocID = Hoadonnuoc.HoadonnuocID join [HoaDonHaDong].[dbo].Khachhang on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID  WHERE Hoadonnuoc.ThangHoaDon =" + now.Month + " and Hoadonnuoc.NamHoaDon = " + now.Year + " and Khachhang.QuanhuyenID =" + chinhanh + " ;";
                //    var updateSotien = db.Database.ExecuteSqlCommand(sql2);

                //    var hoadonnuocs = (from c1 in db.Hoadonnuocs
                //                       join c2 in db.DuCoes on c1.SoTienNopTheoThang.ID equals c2.TienNopTheoThangID
                //                       where c1.Khachhang.QuanhuyenID == chinhanh && (c1.Trangthaithu != true || c1.Trangthaithu == null)
                //                       && (c1.ThangHoaDon.Value == now.Month && c1.NamHoaDon == now.Year)
                //                       select new Models.HoaDonDayDu { h = c1, d = c2 });
                //    hoadonnuocs.ToList().ForEach(hd =>
                //    {
                //        if (hd.d.SoTienDu > hd.h.SoTienNopTheoThang.SoTienPhaiNop)
                //        { hd.d.SoTienDu = hd.d.SoTienDu - Convert.ToInt32(hd.h.SoTienNopTheoThang.SoTienPhaiNop); }
                //    });

                //    var sql3 = "INSERT INTO [dbo].[GiaoDich]([TienNopTheoThangID],[NgayGiaoDich],[SoTien],[SoDu]) SELECT  [ID] ,CAST(GETDATE() AS DATE),[SoTienPhaiNop],SoTienDu FROM [HoaDonHaDong].[dbo].[SoTienNopTheoThang] join [HoaDonHaDong].[dbo].[Hoadonnuoc] on Hoadonnuoc.SoTienNopTheoThangID = SoTienNopTheoThang.id left join [HoaDonHaDong].[dbo].[DuCo] on SoTienNopTheoThang.ID = DuCo.TienNopTheoThangID join [HoaDonHaDong].[dbo].[Khachhang] on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID WHERE  ThangHoaDon =" + now.Month + " and NamHoaDon = " + now.Year + "  and Khachhang.QuanhuyenID = " + chinhanh + ";";
                //    var themgiaodich = db.Database.ExecuteSqlCommand(sql3);
                //    var updateNull = db.Database.ExecuteSqlCommand("UPDATE [dbo].[GiaoDich] SET [SoDu] = 0 WHERE [SoDu] is null");

                //    uow.Commit();
                //} catch (Exception e)
                //{
                //    uow.RollBack();
                //}
            }

            pager.ApplyPager(ref items);

            #region view data
            title = "Quản lý công nợ";
            var sectionTitle = "Quản lý công nợ khách hàng ";

            if (filter.HinhThucThanhToan == EHinhThucThanhToan.ChuyenKhoan)
                sectionTitle += "Chuyển khoản";
            else if (filter.LoaiKhachHang == ELoaiKhachHang.HoGiaDinh)
                sectionTitle += "Hộ gia đình";
            else
                sectionTitle += "Cơ quan, tổ chức";

            // load ds to thuoc filter.quanhuyen
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            ViewBag.SectionTitle = sectionTitle;
            #endregion
            return View(items.ToList());
        }
        
    }
}