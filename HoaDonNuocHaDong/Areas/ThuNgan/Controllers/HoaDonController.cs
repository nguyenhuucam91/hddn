using HDNHD.Core.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Globalization;
using System.Linq;
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
        /// view list of HoaDon with filter
        /// </summary>
        public ActionResult Index(HoaDonFilterModel filter, HDNHD.Core.Models.Pager pager, String todo)
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
            var items = hoaDonRepository.GetAllHoaDonModel();
            filter.ApplyFilter(ref items);

            // apply actions
            if (todo == "DanhDauTatCa")
            {
                foreach (var item in items)
                {
                    HoaDonHelpers.ThanhToan(item, uow);
                }
            }

            pager.ApplyPager(ref items);

            #region view data
            title = "Quản lý công nợ khách hàng";

            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion
            return View(items.ToList());
        }

        public ActionResult ThemVaoDuCo(int hoaDonID)
        {


            ViewBag.HoaDonID = hoaDonID;
            return View();
        }

        [HttpPost]
        public AjaxResult ThemVaoDuCo(int hoaDonID, int soTien, string ngayNop)
        {
            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();

            DateTime dt;
            if (!DateTime.TryParseExact(ngayNop,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt))
            {
                dt = DateTime.Now;
            }

            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);
            
            HDNHD.Models.DataContexts.GiaoDich giaoDich = new HDNHD.Models.DataContexts.GiaoDich()
            {
                TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                NgayGiaoDich = dt,
                SoTien = soTien,
                SoDu = soTien
            };

            model.DuCo = model.DuCo ?? new HDNHD.Models.DataContexts.DuCo()
            {
                KhachhangID = model.KhachHang.KhachhangID,
                TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                SoTienDu = 0
            };

            if (soTien > 0)
            {
                model.DuCo.SoTienDu += soTien;
            }

            model.SoTienNopTheoThang.SoTienDaThu += giaoDich.SoTien;
            model.DuCo.SoTienDu += giaoDich.SoDu;

            if (model.DuCo.SoTienDu > 0)
                duCoRepository.Insert(model.DuCo);
            giaoDichRepository.Insert(giaoDich);

            uow.SubmitChanges();

            return AjaxResult.Success("");
        }
    }
}