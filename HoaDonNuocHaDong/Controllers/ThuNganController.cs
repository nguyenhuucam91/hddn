using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class ThuNganController : BaseController
    {
        
        //
        // GET: /ThuNgan/
        //hien thi ra danh sach khach hang ma hoa don có trang thai in =1(đã in)
        public ActionResult DanhSachKhachHangDaInXongHoaDon()
        {
            var hoadon = db.Hoadonnuocs.Where(p => p.Trangthaiin == true);
            List<Tuyenkhachhang> lstTuyen = new List<Tuyenkhachhang>();
            foreach (var hdnuoc in db.Hoadonnuocs.OrderBy(p => p.HoadonnuocID).ToList())
            {
                Tuyenkhachhang t = hdnuoc.Khachhang.Tuyenkhachhang;
                if (hdnuoc.Trangthaiin == true && lstTuyen.FindIndex(x => x.TuyenKHID == t.TuyenKHID) == -1)
                    lstTuyen.Add(t);
            }
            if (Request.QueryString["id"] == null)
                ViewBag.hoadon = hoadon.ToList();
            else
            {
                List<Hoadonnuoc> lstHoadon = new List<Hoadonnuoc>();
                foreach (var hdnuoc in db.Hoadonnuocs.OrderBy(p => p.HoadonnuocID).ToList())
                {
                    if (hdnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == int.Parse(Request.QueryString["id"]))
                        lstHoadon.Add(hdnuoc);
                }
                ViewBag.hoadon = lstHoadon;
            }
            ViewBag.tuyen = lstTuyen;
            return View();
        }
    }
}