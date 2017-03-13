using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class DefaultController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}