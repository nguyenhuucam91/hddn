using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.InHoaDon.Controllers
{
    public class BaoCaoController : BaseController
    {
        public ActionResult Index()
        {
            #region view data
            title = "Báo cáo in hóa đơn";
            #endregion

            return View();
        }

        public ActionResult HoaDonNhan()
        {
            return View();
        }
    }
}