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
        public ActionResult Index(TuyenKhachHangFilterModel filter, Pager pager)
        {
            ITuyenKHRepository tuyenKHRepository = uow.Repository<TuyenKHRepository>();
            
            var items = tuyenKHRepository.GetAll().Select(m => new TuyenKhachHangModel()
            {
                TuyenKhachHang = m
            });
            
            filter.ApplyFilter(ref items);
            pager.ApplyPager(ref items);

            #region view data
            title = "Danh sách tuyến khách hàng đã có chỉ số";
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion

            return View(items.ToList());
        }
    }
}