using HDNHD.Core.Constants;
using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Linq;
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
        /// báo cáo dư có theo tháng
        /// </summary>
        public ActionResult DuCo(int? month, int? year, DuCoFilterModel filter, Pager pager, ViewMode viewMode = ViewMode.Default)
        {
            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            IToRepository toRepository = uow.Repository<ToRepository>();

            // default values
            var current = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = current.Month;
            if (year == null)
                year = current.Year;

            if (filter.Mode == null) // not in filter
            {
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
            var items = duCoRepository.GetAllDuCoModel(month.Value, year.Value);
            items = filter.ApplyFilter(items);

            ViewBag.TongSoDu = items.Sum(m => m.SoTien) ?? 0;

            if (viewMode == ViewMode.Excel)
                return ExcelResult("DuCoExport", items.ToList());

            items = pager.ApplyPager(items);

            #region view data
            title = "Báo cáo dư có";
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;
            #endregion

            return View(items.ToList());
        }

        /// <summary>
        /// báo cáo dư nợ tính đến tháng hiện tại
        /// </summary>
        public ActionResult DuNo(int? month, int? year, DuNoFilterModel filter, Pager pager, ViewMode viewMode = ViewMode.Default)
        {
            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();
            IToRepository toRepository = uow.Repository<ToRepository>();

            // default values
            var current = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = current.Month;
            if (year == null)
                year = current.Year;
            
            if (filter.Mode == null) // not in filter
            {
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

            var items = hoaDonRepository.GetAllDuNoModel(month.Value, year.Value);
            items = filter.ApplyFilter(items);

            ViewBag.TongSoTienPhaiNop = (long) (items.Sum(m => m.SoTienNopTheoThang.SoTienPhaiNop) ?? 0);
            ViewBag.TongSoTienDaNop = items.Sum(m => m.SoTienDaNop) ?? 0;
            ViewBag.TongSoTienNo = items.Sum(m => m.SoTienNo) ?? 0;

            if (viewMode == ViewMode.Excel)
                return ExcelResult("DuNoExport", items.ToList());

            items = pager.ApplyPager(items);

            #region view data 
            title = "Báo cáo dư nợ";
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;
            
            #endregion
            return View(items.ToList());
        }

        /// <summary>
        ///     Revenue report for month m / year y
        ///         DuCoDauKy = sum DuCo of prev month
        ///         DuNoDauKy = sum HoaDon of prev month with TrangThaiThu = false ???
        /// </summary>
        public ActionResult DoanhThu(int? month, int? year)
        {
            // default values
            var current = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = current.Month;
            if (year == null)
                year = current.Year;

            //if (filter.Mode == null) // not in filter
            //{
            //    // set selected to, quan huyen = nhanVien's to, quan huyen
            //    if (nhanVien != null)
            //    {
            //        filter.NhanVienID = nhanVien.NhanvienID;
            //        filter.ToID = nhanVien.ToQuanHuyenID;

            //        var to = toRepository.GetByID(nhanVien.ToQuanHuyenID ?? 0);
            //        if (to != null)
            //        {
            //            filter.QuanHuyenID = to.QuanHuyenID;
            //        }
            //    }
            //}


            // data
            var prev = current.AddMonths(-1);
            
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            

            #region viewdata
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;
            #endregion
            
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