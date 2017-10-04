using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Models.KhachHang;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class DefaultController : BaseController
    {
        protected BackupRepository backupRepository = new BackupRepository();
        public ActionResult Index()
        {
            DateTime currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
           
            var currentDate = DateTime.Now;
            ModelKhachHang modelKH = new ModelKhachHang();
            modelKH.updateKHHetHanApGia(currentDate);
            title = "Công ty TNHH một thành viên nước sạch Hà Đông";            
            return View();
        }

        public void getControllersActions()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)));
        }
	}
}