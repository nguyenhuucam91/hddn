using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
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
    public class BaoCaoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// báo cáo khách hàng đóng tiền dư tính đến tháng hiện tại
        /// </summary>
        public ActionResult DuCo(DuCoFilterModel filter, Pager pager)
        {
            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            IToRepository toRepository = uow.Repository<ToRepository>();

            // default values
            if (filter.Mode == null) // not in filter
            {
                if (filter.Year == null)
                    filter.Year = DateTime.Now.Year;
                if (filter.Month == null)
                    filter.Year = DateTime.Now.Month;

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

            var items = duCoRepository.GetAll().Select(m => new DuCoModel() { 
                DuCo = m
            });

            filter.ApplyFilter(ref items, ref pager);

            #region view data
            title = "Báo cáo dư có";
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion

            return View(items.ToList());
        }

        /// <summary>
        /// báo cáo  nợ theo tháng
        /// mặc định tháng, năm hiện tại, sao khi chọn tháng từ form, dữ get request dc gửi dến controller
        /// </summary>
        public ActionResult DuNo(DuNoFilterModel filter, Pager pager)
        {
            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();
            var items = hoaDonRepository.GetAll().Select(m => new DuNoModel()
            {
                HoaDon = m
            });

            filter.ApplyFilter(ref items, ref pager);

            #region view data 
            title = "Báo cáo dư nợ";
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion

            return View(items.ToList());
        }

        /// <summary>
        /// Báo cáo doanh thu theo tháng
        /// </summary>
        public ActionResult DoanhThu(int? Year, int? Month)
        {
            //var month = DateTime.Now.Month;
            //var year = DateTime.Now.Year;
            //if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString().Length > 0)
            //{
            //    year = Convert.ToInt32(Request.QueryString["year"]);
            //}
            //if (Request.QueryString["month"] != null && Request.QueryString["month"].ToString().Length > 0)
            //{
            //    month = Convert.ToInt32(Request.QueryString["month"]);
            //}
            //var prevMonth = month-1;
            //var prevYear = year;
            //if (month == 1)
            //{
            //    prevMonth = 12;
            //    prevYear = year - 1;
            //}
            //var DuCoDauKy = 0;
            //var NoDauKy = 0;
            ////var NoDauKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == prevMonth && h.NamHoaDon == prevYear).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            ////var DuCoDauKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == prevMonth && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == prevYear).Sum(h => h.SoTienDu);
            //var NoCuoiKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            //var DuCoCuoiKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year).Sum(h => h.SoTienDu);
            //var HoaDonInTrongThang = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            //var DoanhThuPhaiThu = HoaDonInTrongThang+NoDauKy-DuCoDauKy;
            //var DoanhThuThang = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && h.Trangthaithu == true).Sum(h => h.SoTienNopTheoThang.SoTienDaThu);

            //if (System.IO.File.ReadAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), System.Text.Encoding.Unicode).Length == 0)
            //{
            //    DuCoDauKy = 0;
            //    NoDauKy = 0;
            //}
            //else
            //{
            //    String[] f = null;
            //    var lines = System.IO.File.ReadAllLines(Server.MapPath(@"~/Controllers/doanhthu.txt"));
            //    for (int i = lines.Length - 1; i >= 0; i--)
            //    {
            //        var br = lines[i].Split(' ');
            //        if (br[0] == prevMonth.ToString() && br[1] == prevYear.ToString())
            //        {
            //            f = br;
            //            break;
            //        }
            //    }
            //    if (f == null)
            //    {
            //        NoDauKy = 0;
            //        DuCoDauKy = 0;
            //    }
            //    else
            //    {
            //        NoDauKy = Convert.ToInt32(f[2]);
            //        DuCoDauKy = Convert.ToInt32(f[3]);
            //    }
            //}
          
            //List<Object> doanhthu = new List<Object>();
            //doanhthu.Add(NoDauKy);
            //doanhthu.Add(DuCoDauKy);
            //doanhthu.Add(DoanhThuPhaiThu);
            //doanhthu.Add(DoanhThuThang);
            //doanhthu.Add(NoCuoiKy);
            //doanhthu.Add(DuCoCuoiKy);
            //doanhthu.Add(month);
            //doanhthu.Add(year);
            //doanhthu.Add(HoaDonInTrongThang);
            //ViewBag.DoanhThu = doanhthu;
         
            //return View();
            return View();
        }
    }
}