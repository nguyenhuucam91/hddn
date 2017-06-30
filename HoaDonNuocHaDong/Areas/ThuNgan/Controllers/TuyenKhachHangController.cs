using HDNHD.Core.Constants;
using HDNHD.Core.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Linq;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class TuyenKhachHangController : BaseController
    {
        public ActionResult Index(TuyenKhachHangFilterModel filter, Pager pager, ViewMode viewMode = ViewMode.Default)
        {
            title = "Danh sách tuyến khách hàng";
            
            ITuyenKHRepository tuyenKHRepository = uow.Repository<TuyenKHRepository>();

            var items = tuyenKHRepository.GetAll();
            
            items = filter.ApplyFilter(items);

            if (viewMode == ViewMode.Excel)
                return ExcelResult("IndexExport", items.ToList());
            if (viewMode == ViewMode.Print)
                return View("IndexPrint", items.ToList());

            items = pager.ApplyPager(items);

            #region view data
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion

            return View(items.ToList());
        }
    }
}