using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Models;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class NhapChiSoCapController : BaseController
    {
        //
        // GET: /NhapChiSoCap/
        public ActionResult Index()
        {
            ControllerBase<NhapChiSoCap> cb = new ControllerBase<NhapChiSoCap>();
            List<NhapChiSoCap> lst = cb.Query("Select ChinhanhID,Ten from Chinhanh");
            ViewData["lst"] = lst;
            return View();
        }

        [HttpPost]
        public ActionResult NhapChiSoCap(FormCollection fc)
        {
            int chinhanhID = int.Parse(fc["s1"]);
            int thang = int.Parse(fc["m1"]);
            int nam = int.Parse(fc["y1"]);
            ControllerBase<MTuyenong> cb = new ControllerBase<MTuyenong>();
            List<MTuyenong> lst = cb.Query("Select Tuyenong.TuyenongID,Tuyenong.Tentuyen,Captuyen.ten from Tuyenong inner join Captuyen on Tuyenong.CaptuyenID = Captuyen.CaptuyenID where ChinhanhID = "+chinhanhID);
            ControllerBase<NhapChiSoCap> cbNhap = new ControllerBase<NhapChiSoCap>();
            List<NhapChiSoCap> lstNhap = cbNhap.Query("Select ChinhanhID,Ten from Chinhanh where ChinhanhID = " + chinhanhID);
           
            ViewBag.chinhanh=lstNhap[0].Ten;
            ViewData["lst"] = lst;
            ViewBag.thang = thang;
            ViewBag.nam = nam;

            return View();
        }
        [HttpPost]
        public ActionResult NhapChiSoCapThanhCong(FormCollection fc)
        {
            ControllerBase<ChisocapModel> chisoCb = new ControllerBase<ChisocapModel>();
            chisoCb.InsertProcName = "Chisocap_ProcInsert";
            var a = fc["TuyenongID0"];
            int i = 0;
            while (fc["TuyenongID" + i] != null)
            {
                ChisocapModel csc = new HoaDonNuocHaDong.Models.ChisocapModel
                { 
                    TuyenongID = int.Parse(fc["TuyenongID" + i]), 
                    Thang = int.Parse(fc["Thang" + i]), 
                    Nam = int.Parse(fc["Nam" + i]), 
                    Chiso = int.Parse(fc[i.ToString()]) };
                chisoCb.Insert(csc);
                i++;
            }

            return View();
        }
	}
}