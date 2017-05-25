using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.Admin.Controllers
{
    public class DatabaseController : BaseController
    {
        public ActionResult Restore()
        {
            title = "Khôi phục dữ liệu";
            return View();
        }
    }
}