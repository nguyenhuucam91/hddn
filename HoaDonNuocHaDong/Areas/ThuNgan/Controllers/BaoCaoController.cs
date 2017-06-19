using HDNHD.Core.Constants;
using HDNHD.Core.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using HDNHD.Core.Repositories.Interfaces;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class BaoCaoController : BaseController
    {
        private IToRepository toRepository;

        public BaoCaoController()
        {
            toRepository = uow.Repository<ToRepository>();
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// báo cáo dư có theo tháng
        /// </summary>
        public ActionResult DuCo(int? month, int? year, DuCoFilterModel filter, Pager pager, ViewMode viewMode = ViewMode.Default)
        {
            title = "Báo cáo dư có";

            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            
            // default values: xem báo cáo tháng trước
            var dtBaoCao = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = dtBaoCao.Month;
            if (year == null)
                year = dtBaoCao.Year;

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
            var items = duCoRepository.GetAllDuCoModel(month.Value, year.Value);
            items = filter.ApplyFilter(items);

            ViewBag.TongSoDu = items.Sum(m => m.SoTien) ?? 0;
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;

            if (viewMode == ViewMode.Excel)
                return ExcelResult("DuCoExport", items.ToList());
            if (viewMode == ViewMode.Print)
                return View("DuCoPrint", items.ToList());

            items = pager.ApplyPager(items);

            #region view data
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion

            return View(items.ToList());
        }

        /// <summary>
        /// báo cáo dư nợ tính đến tháng hiện tại
        /// </summary>
        public ActionResult DuNo(int? month, int? year, DuNoFilterModel filter, Pager pager, ViewMode viewMode = ViewMode.Default)
        {
            title = "Báo cáo dư nợ";

            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();

            // default values: xem báo cáo tháng trước
            var dtBaoCao = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = dtBaoCao.Month;
            if (year == null)
                year = dtBaoCao.Year;
            
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

            var items = hoaDonRepository.GetAllDuNoModel(month.Value, year.Value);
            items = filter.ApplyFilter(items);

            ViewBag.TongSoTienPhaiNop = (long) (items.Sum(m => m.SoTienNopTheoThang.SoTienPhaiNop) ?? 0);
            ViewBag.TongSoTienDaNop = items.Sum(m => m.SoTienDaNop) ?? 0;
            ViewBag.TongSoTienNo = items.Sum(m => m.SoTienNo) ?? 0;
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;

            if (viewMode == ViewMode.Excel)
                return ExcelResult("DuNoExport", items.ToList());
            if (viewMode == ViewMode.Print)
                return View("DuNoPrint", items.ToList());

            items = pager.ApplyPager(items);

            #region view data 
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion
            return View(items.ToList());
        }

        /// <summary>
        ///     Revenue report for month m / year y
        ///         DuCoDauKy = sum DuCo of prev month
        ///         DuNoDauKy = sum HoaDon of prev month with TrangThaiThu = false ???
        /// </summary>
        public ActionResult DoanhThu(int? month, int? year, DoanhThuFilterModel filter, GiaoDichFilterModel giaoDichFilter, 
            DuCoFilterModel duCoFilter, DuNoFilterModel duNoFilter, SoTienNopTheoThangFilterModel soTienNopTheoThangFilter, ViewMode viewMode = ViewMode.Default)
        {
            title = "Báo cáo doanh thu";

            var hoaDonRepository = uow.Repository<HoaDonRepository>();
            var duCoRepository = uow.Repository<DuCoRepository>();
            var giaoDichRepository = uow.Repository<GiaoDichRepository>();
            var soTienNopTheoThangRepository = uow.Repository<SoTienNopTheoThangRepository>();

            // default values: xem báo cáo tháng trước
            var dtBaoCao = DateTime.Now.AddMonths(-1);
            if (month == null)
                month = dtBaoCao.Month;
            if (year == null)
                year = dtBaoCao.Year;

            if (filter.Mode == null) // not in filter
            {
                // set selected to, quan huyen = nhanVien's to, quan huyen
                if (nhanVien != null)
                {
                    //filter.NhanVienID = nhanVien.NhanvienID;
                    //filter.ToID = nhanVien.ToQuanHuyenID;

                    var to = toRepository.GetByID(nhanVien.ToQuanHuyenID ?? 0);
                    if (to != null)
                    {
                        filter.QuanHuyenID = to.QuanHuyenID;
                    }
                }

                giaoDichFilter.NhanVienID = filter.NhanVienID;
                giaoDichFilter.ToID = filter.ToID;
                giaoDichFilter.QuanHuyenID = filter.QuanHuyenID;

                duCoFilter.NhanVienID = filter.NhanVienID;
                duCoFilter.ToID = filter.ToID;
                duCoFilter.QuanHuyenID = filter.QuanHuyenID;

                duNoFilter.NhanVienID = filter.NhanVienID;
                duNoFilter.ToID = filter.ToID;
                duNoFilter.QuanHuyenID = filter.QuanHuyenID;

                soTienNopTheoThangFilter.NhanVienID = filter.NhanVienID;
                soTienNopTheoThangFilter.ToID = filter.ToID;
                soTienNopTheoThangFilter.QuanHuyenID = filter.QuanHuyenID;
            }

            // data
            var prevMonth = month;
            var prevYear = year;
            prevMonth--;
            if (prevMonth < 1)
            {
                prevMonth = 12;
                prevYear--;
            }
            
            // dư nợ đầu kỳ
            var duNoDauKy = hoaDonRepository.GetAllDuNoModel(prevMonth.Value, prevYear.Value);
            duNoDauKy = duNoFilter.ApplyFilter(duNoDauKy);
            ViewBag.DuNoDauKy = duNoDauKy.Sum(m => m.SoTienNo) ?? 0;

            // dư có đầu kỳ
            var duCoDauKy = duCoRepository.GetAllDuCoModel(prevMonth.Value, prevYear.Value);
            duCoDauKy = duCoFilter.ApplyFilter(duCoDauKy);
            ViewBag.DuCoDauKy = duCoDauKy.Sum(m => m.SoTien) ?? 0;

            // hóa đơn in trong tháng (tháng hóa đơn - 1)
            var soTienNopTheoThang = soTienNopTheoThangRepository.GetAllByMonthYear(month.Value, year.Value);
            soTienNopTheoThang = soTienNopTheoThangFilter.ApplyFilter(soTienNopTheoThang);
            ViewBag.SoTienTrenHoaDon = soTienNopTheoThang.Sum(m => m.SoTienTrenHoaDon) ?? 0;
            
            // số tiền phải thu
            ViewBag.SoTienPhaiThu = soTienNopTheoThang.Sum(m => m.SoTienPhaiNop) ?? 0;

            // số tiền đã thu 
            var giaoDich = giaoDichRepository.GetAllByMonthYear(month.Value, year.Value);
            giaoDich = giaoDichFilter.ApplyFilter(giaoDich);
            ViewBag.SoTienDaThu = giaoDich.Sum(m => m.SoTien) ?? 0;
            ViewBag.SoTienDaThuChuyenKhoan = giaoDich.Where(m => m.IsChuyenKhoan).Sum(m => m.SoTien) ?? 0;
            ViewBag.SoTienDaThuTienMat = ViewBag.SoTienDaThu - ViewBag.SoTienDaThuChuyenKhoan;

            // dư nợ cuối kỳ
            var duNoCuoiKy = hoaDonRepository.GetAllDuNoModel(month.Value, year.Value);
            duNoCuoiKy = duNoFilter.ApplyFilter(duNoCuoiKy);
            ViewBag.DuNoCuoiKy = duNoCuoiKy.Sum(m => m.SoTienNo) ?? 0;

            // dư có cuối kỳ
            var duCoCuoiKy = duCoRepository.GetAllDuCoModel(month.Value, year.Value);
            duCoCuoiKy = duCoFilter.ApplyFilter(duCoCuoiKy);
            ViewBag.DuCoCuoiKy = duCoCuoiKy.Sum(m => m.SoTien) ?? 0;

            #region viewdata
            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;
            ViewBag.Filter = filter;
            #endregion

            if (viewMode == ViewMode.Excel)
                return ExcelResult("DoanhThuExport");

            return View();
        }
    }
}