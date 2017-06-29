using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HoaDonNuocHaDong;
using System.Web.Routing;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using HoaDonNuocHaDong.Helper;
using System.Data.Entity.Validation;
using HoaDonNuocHaDong.Base;

namespace HoaDonNuocHaDong.Controllers
{
    public class CongnoController : Controller
    {
        private NguoidungHelper ngDungHelper = new NguoidungHelper();
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["tenDangNhap"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }

        }

        /// <summary>
        /// Hàm chạy sau khi controller và action chạy xong
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            //catch controller action
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = String.Concat(this.ControllerContext.RouteData.Values["controller"].ToString(), "Controller");

            //lấy chức năng dựa trên tên action và controller
            var chucNangID = (from i in db.Chucnangchuongtrinhs
                              join r in db.Nhomchucnangs on i.NhomchucnangID equals r.NhomchucnangID
                              where i.TenAction == actionName && r.TenController == controllerName
                              select new
                              {
                                  ChucNangID = i.ChucnangID,
                              }).FirstOrDefault();
            if (chucNangID != null)
            {
                Lichsusudungct lichSu = new Lichsusudungct();
                lichSu.ChucnangID = Convert.ToInt32(chucNangID.ChucNangID);
                lichSu.NguoidungID = Convert.ToInt32(Session["nguoiDungID"].ToString());
                lichSu.Thoigian = DateTime.Now;
                db.Lichsusudungcts.Add(lichSu);
                db.SaveChanges();
            }


            //đầu tháng sẽ ghi lại dư nợ và dư có đầu kỳ vào file doanh thu.txt
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var prevMonth = month - 1;
            var prevYear = year;
            if (month == 1)
            {
                prevMonth = 12;
                prevYear = year - 1;
            }
            var firstday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstday.AddMonths(1).AddDays(-1);
            //kiểm tra nút đánh dấu tất cả khách hàng đã nọp tiền được nhấn chưa
            bool ex = System.IO.File.Exists(Server.MapPath(@"~/Controllers/isChecked.txt"));
            //kiểm tra file isChecked.txt đã có chưa
            if (!System.IO.File.Exists(Server.MapPath(@"~/Controllers/isChecked.txt")))
            {
                var chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
                var quanhuyen = db.Quanhuyens.ToList();
                System.IO.File.WriteAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), "");
                //lưu vào file isChecked các chuỗi gồm tháng, quận huyện id, 1/0 với 1 là dc check đánh dấu tất cả còn 0 là k
                foreach (Quanhuyen q in quanhuyen)
                {
                    System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), Environment.NewLine + DateTime.Now.Month + " " + q.QuanhuyenID + " 1");
                }

                DirectoryInfo dInfo = new DirectoryInfo(Server.MapPath(@"~/Controllers/isChecked.txt"));
                System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule("everyone", System.Security.AccessControl.FileSystemRights.FullControl,
                                                                 System.Security.AccessControl.InheritanceFlags.ObjectInherit | System.Security.AccessControl.InheritanceFlags.ContainerInherit,
                                                                 System.Security.AccessControl.PropagationFlags.NoPropagateInherit, System.Security.AccessControl.AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            else
            {
                String m = "";
                var chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
                var isChecked = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/isChecked.txt")).Last().ToString();
                String[] ch = isChecked.Split(' ');
                m = ch[0];

                //nếu đã sang tháng mới, ghi lại thông tin và cho tất cả là dc phép thay đổi
                if (m != month.ToString())
                {
                    var quanhuyen = db.Quanhuyens.ToList();
                    System.IO.File.WriteAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), "");
                    foreach (Quanhuyen q in quanhuyen)
                    {
                        System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), Environment.NewLine + DateTime.Now.Month + " " + q.QuanhuyenID + " 1");
                    }
                }
            }
            //ghi doanh thu vào file doanh thu.txt
            //1 dòng doanh thu bao gồm tháng, năm, số dư nợ và dư có
            String[] check = null;
            var ac = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/doanhthu.txt")).ToList();
            if (ac != null && ac.Count() > 0)
            {
                String fc = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/doanhthu.txt")).ToList().Last();
                check = fc.Split(' ');
                bool newmonth = check[0] != month.ToString();
            }
            if (check == null || check[0] != month.ToString())
            {

                var NoCuoiKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
                var DuCoCuoiKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year).Sum(h => h.SoTienDu);
                String doanhthuthang = Environment.NewLine + month + " " + year + " " + NoCuoiKy + " " + DuCoCuoiKy;
                System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), doanhthuthang);
            }
            if (DateTime.Now == lastDayOfMonth)
            {
                var NoCuoiKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
                var DuCoCuoiKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year).Sum(h => h.SoTienDu);
                if (NoCuoiKy == null)
                {
                    NoCuoiKy = 0;
                }
                if (DuCoCuoiKy == null)
                {
                    DuCoCuoiKy = 0;
                }
                if (System.IO.File.ReadAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), System.Text.Encoding.Unicode).Length == 0)
                {
                    String doanhthuthang = Environment.NewLine + month + " " + year + " " + NoCuoiKy + " " + DuCoCuoiKy;
                    System.IO.File.WriteAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), doanhthuthang);
                }
                else
                {
                    String f = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/doanhthu.txt")).Last();
                    var a = f.Split(' ');
                    String doanhthuthang = month + " " + year + " " + NoCuoiKy + " " + DuCoCuoiKy;
                    if (a[0] != month.ToString())
                    {
                        System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), doanhthuthang);
                    }
                }
            }
        }

        // GET: /Hoadon/
        //mặc định loại khách hàng có ID là 1 sẽ là hộ gia đình
        public ActionResult Index()
        {
            var chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hoadonnuocs = db.Hoadonnuocs.Take(0);

            int phongBanID = Convert.ToInt32(Session["phongBan"]);
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = new List<Tuyenkhachhang>();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            int fff = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList().Count();
            return View(hoadonnuocs.ToList());
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            //nếu tiêu chí tìm kiếm được nhập
            Boolean tim = false;
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            //lấy danh sách các hóa đơn  của khách hàng hộ gia đình theo tuyến -  
            //lấy cả hóa đơn của tháng trước trong trường hợp tháng này chưa có hóa đơn
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.LoaiKHID == KhachHang.SINHHOAT && p.Khachhang.TuyenKHID == tuyenID && p.Trangthaiin == true
                && (p.ThangHoaDon.Value == now.Month && p.NamHoaDon == now.Year || p.Trangthaithu == false || p.Trangthaithu == null)
                ).OrderBy(p => p.Khachhang.TTDoc); 
            // nếu tuyến chưa được chọn, tìm hóa đơn theo form địa chỉ, tên, mã
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) || 
                        (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc); 
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p=>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) || 
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth))
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                        //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.Diachi.ToLower().Contains(diachi)
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                   
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH)  && p.Khachhang.LoaiKHID == KhachHang.SINHHOAT && 
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) && 
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) && 
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) || 
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc); 
                    }
                }
            }
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.selectedTuyen = tuyenID;           
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            //sử dụng danh sách ViewBag để load danh sách ra dropdownList
            filterViewBagList();           
            if (tim)
            {
                return View(hoadonnuocs.Take(200).ToList());
            }
            return View(hoadonnuocs.ToList());
        }

        /// <summary>
        /// Danh sách ViewBag cho phần lọc theo tổ, nhân viên, tuyến của thu ngân
        /// </summary>
        public void filterViewBagList()
        {
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.selectedChiNhanh = quanHuyenID;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            int phongBanID = Convert.ToInt32(Session["phongBan"]);
            List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.to = toLs;
            //load danh sách nhân viên thuộc tổ có phòng ban đó.
            List<Nhanvien> nVLs = new List<Nhanvien>();
            foreach (var item in toLs)
            {
                List<Nhanvien> _nvLs = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == item.ToQuanHuyenID && (p.IsDelete == false || p.IsDelete == null) && p.PhongbanID == PhongbanHelper.THUNGAN).ToList();
                nVLs.AddRange(_nvLs);
            }
            ViewBag.nhanVien = nVLs;
            //load danh sách tuyến thuộc nhân viên đó.
            List<Tuyenkhachhang> tuyensLs = new List<Tuyenkhachhang>();
            foreach (var item in nVLs)
            {
                var tuyenTheoNhanVien = (from i in db.Tuyentheonhanviens
                                         join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                         join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                         join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                         where i.NhanVienID == item.NhanvienID
                                         select r).ToList();
                tuyensLs.AddRange(tuyenTheoNhanVien);
            }
            ViewBag.tuyen = tuyensLs;
        }

        public ActionResult KhachHangGiaDinhNo()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var hoadonnuocs = db.Hoadonnuocs.Take(0);

            //lọc
            
            filterViewBagList();
            return View(hoadonnuocs.ToList());
        }

        [HttpPost]
        public ActionResult KhachHangGiaDinhNo(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            Boolean tim = false;
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            //lấy danh sách các hóa đơn  của khách hàng hộ gia đình theo tuyến -  
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                p.Khachhang.TuyenKHID == tuyenID &&
                (p.Trangthaithu == false || p.Trangthaithu == null)).OrderBy(p => p.Khachhang.TTDoc);
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) ||
                        (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth))
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                           p.Khachhang.Diachi.ToLower().Contains(diachi)
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) &&
                            p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            //lọc
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).OrderBy(p => p.Ten).ToList();
            ViewBag.tim = tim;
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            if (tim)
            {
                return View(hoadonnuocs.Take(200).ToList());
            }
            return View(hoadonnuocs.ToList());
        }

        public ActionResult KhachHangDoanhNghiepNo()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hoadonnuocs = db.Hoadonnuocs.Take(0);

            //lọc
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false && p.IsDelete == true).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }

        [HttpPost]
        public ActionResult KhachHangDoanhNghiepNo(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            Boolean tim = false;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            //lấy danh sách các hóa đơn  của khách hàng doanh nghiệp theo tuyến -  
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.LoaiKHID != KhachHang.SINHHOAT &&
                p.Khachhang.TuyenKHID == tuyenID &&
                (p.Trangthaithu == false || p.Trangthaithu == null)).OrderBy(p => p.Khachhang.TTDoc);
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) &&
                        p.Khachhang.LoaiKHID != KhachHang.SINHHOAT &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) || (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year)))
                        .OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.LoaiKHID != KhachHang.SINHHOAT
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                           p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.LoaiKHID != KhachHang.SINHHOAT
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) &&
                            p.Khachhang.LoaiKHID != KhachHang.SINHHOAT &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            p.Khachhang.LoaiKHID != KhachHang.SINHHOAT &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).OrderBy(p => p.Ten).ToList();
            ViewBag.selected = tuyenID;
            ViewBag.tim = tim;
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            if (tim == true)
            {
                return View(hoadonnuocs.Take(200).ToList());
            }
            return View(hoadonnuocs.ToList());
        }

        public ActionResult KhachHangChuyenKhoanNo()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hoadonnuocs = db.Hoadonnuocs.Take(0);

            //lọc
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false && p.IsDelete == true).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }


        [HttpPost]
        public ActionResult KhachHangChuyenKhoanNo(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            Boolean tim = false;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.HinhthucttID == 2 && p.Khachhang.TuyenKHID == tuyenID
                 && (p.Trangthaithu == false || p.Trangthaithu == null)).OrderBy(p => p.Khachhang.TTDoc);
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) &&
                        p.Khachhang.HinhthucttID == 2 &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) || (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year)))
                        .OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.HinhthucttID == 2
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                           p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.HinhthucttID == 2
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) &&
                            p.Khachhang.HinhthucttID == 2 &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year)))
                            .OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            p.Khachhang.HinhthucttID == 2 &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            ViewBag.tim = tim;
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false && p.IsDelete == false).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }


        /// <summary>
        /// Hàm sử dụng HTTP POST request để cập nhật trạng thái thu cho model
        /// </summary>
        /// <param name="HoadonnuocID">Ajax post data từ Assets/js/CongNo.js</param>
        /// <param name="printStatus">Ajax post data từ Assets/js/CongNo.js</param>
        /// <param name="hoaDonNuoc"></param>
        /// <returns></returns>
        [HttpPost]
        public String ChangeTrangthai(String HoadonnuocID, String printStatus, Hoadonnuoc hoaDonNuoc)
        {
            //lấy chuỗi HoadonnuocID từ ajax request
            int hoaDonNuocID = Convert.ToInt32(HoadonnuocID);
            Hoadonnuoc hoaDon = db.Hoadonnuocs.Where(p => p.HoadonnuocID == hoaDonNuocID).FirstOrDefault();
            if (hoaDon != null && hoaDon.SoTienNopTheoThang != null)
            {
                //nếu trạng thái in từ ajax request là true, i.e tích trạng thái in
                //thêm 1 giao dịch nộp tiền với số tiền đã thu bằng số tiền phải nộp
                if (printStatus.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    hoaDon.Trangthaithu = true;
                    var ngayNop = DateTime.Now.Date;
                    hoaDon.NgayNopTien = ngayNop;
                    String ngay = ngayNop.Day + "/" + ngayNop.Month + "/" + ngayNop.Year;
                    var giaodich = new GiaoDich();
                    giaodich.TienNopTheoThangID = hoaDon.SoTienNopTheoThangID;
                    giaodich.SoTien = (int)hoaDon.SoTienNopTheoThang.SoTienPhaiNop;
                    giaodich.NgayGiaoDich = ngayNop;
                    //dư có là danh sách khách hàng nộp thừa tiền hóa đơn trong tháng đó
                    var duco = db.DuCoes.Where(d => d.TienNopTheoThangID == hoaDon.SoTienNopTheoThangID).FirstOrDefault();

                    if (duco != null)
                    {
                        giaodich.SoDu = duco.SoTienDu;
                    }
                    else
                    {
                        giaodich.SoDu = 0;
                    }
                    int dd = giaodich.SoDu.Value;
                    db.GiaoDiches.Add(giaodich);
                }
                //nếu trạng thái in từ ajax request là false, i.e. bỏ tick trạng thái in
                //xóa hết tất cả các giao dịch trong tháng
                else
                {
                    hoaDon.NgayNopTien = null;
                    hoaDon.Trangthaithu = false;
                    int tientru = 0;
                    var giaodich = db.GiaoDiches.Where(g => g.TienNopTheoThangID == hoaDon.SoTienNopTheoThangID).ToList();
                    foreach (GiaoDich g in giaodich)
                    {
                        if (g.SoDu != null)
                        {
                            tientru = tientru + (int)g.SoDu;
                        }
                        db.GiaoDiches.Remove(g);
                    }
                    var duco = db.DuCoes.Where(d => d.TienNopTheoThangID == hoaDon.SoTienNopTheoThangID).FirstOrDefault();
                    if (duco != null)
                    {
                        if ((duco.SoTienDu.Value - tientru) > 0)
                        {
                            duco.SoTienDu = duco.SoTienDu.Value - tientru;
                        }
                        else
                        {
                            db.DuCoes.Remove(duco);
                        }
                    }
                }
                db.SaveChanges();
            }
            //gửi về ajax 1 chuối chưa thông tin số tiền phải nộp và tiền nộp theo tháng id
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.Khachhang.LoaiKHID == 1 && h.Ngayhoadon.Value > now || h.Trangthaithu == false);
            String n = "";
            if (hoaDon.SoTienNopTheoThang != null)
            {
                n = "" + hoaDon.SoTienNopTheoThang.SoTienPhaiNop.Value + " " + hoaDon.SoTienNopTheoThangID.Value;
            }

            return n;
        }

        //cập nhật loại khách hàng, sử dụng post request
        //parameter được gửi bằng ajax cuối file _LoggedIn.cshtml
        [HttpPost]
        public ActionResult ChangeStt(String KhachhangID, String loaiKH, String action)
        {
            HoaDonHaDongEntities Db = new HoaDonHaDongEntities();


            Khachhang Existed_Emp = Db.Khachhangs.Find(Convert.ToInt32(KhachhangID));
            if (Existed_Emp != null)
            {
                Existed_Emp.LoaiKHID = Convert.ToInt32(loaiKH);
            }
            Db.SaveChanges();
            if (action == "Index")
                return Redirect("Index");
            else if (action == "congNoDoanhNghiep")
                return Redirect("congNoDoanhNghiep");
            else
                return Redirect("congNoChuyenKhoan");
        }

        //hàm thay đổi hình thức thanh toán của khách hàng
        //parameter được gửi bằng ajax cuối file _LoggedIn.cshtml
        [HttpPost]
        public void ChangeHinhThucTT(String KhachhangID, String loaitt, String action)
        {
            HoaDonHaDongEntities Db = new HoaDonHaDongEntities();
            Khachhang Existed_Emp = Db.Khachhangs.Find(Convert.ToInt32(KhachhangID));
            if (Existed_Emp != null)
            {
                int hinhthucthanhtoan = Convert.ToInt32(loaitt);
                Existed_Emp.HinhthucttID = hinhthucthanhtoan;
                if (Existed_Emp.Masothue == null || Existed_Emp.Masothue.Length == 0)
                {
                    Existed_Emp.Masothue = "";
                }
                if (Existed_Emp.Diachithutien == null || Existed_Emp.Diachithutien.Length == 0)
                {
                    Existed_Emp.Diachithutien = "";
                }
                if (Existed_Emp.Sotaikhoan == null || Existed_Emp.Sotaikhoan.Length == 0)
                {
                    Existed_Emp.Sotaikhoan = "";
                }
                if (Existed_Emp.Dienthoai == null || Existed_Emp.Dienthoai.Length == 0)
                {
                    Existed_Emp.Dienthoai = "";
                }
                Db.Entry(Existed_Emp).State = EntityState.Modified;
            }
            try
            {
                Db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Response.Write(validationError.PropertyName + "--" + validationError.ErrorMessage);
                    }
                }
            }
        }

        //cập nhật ngày nộp tiền 
        //parameter dc gửi bằng ajax từ file BaoCao.js
        [HttpPost]
        public void ChangeNgayNop(String HoadonID, String ngaynop)
        {
            int id = Convert.ToInt32(HoadonID);
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("vi-VN");
            DateTime ngayNop = DateTime.ParseExact(ngaynop, "dd/MM/yyyy", culture);
            ngayNop = DateTime.ParseExact(ngayNop.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var hoadon = db.Hoadonnuocs.Find(id);
            hoadon.NgayNopTien = ngayNop;
            db.SaveChanges();
        }

        //cập nhật số tiền đã nộp và tạo record về dư có
        //parameter dc gửi bằng ajax
        [HttpPost]
        public ActionResult NopTien(String TienNopTheoThangID, String soTien, String action)
        {
            HoaDonHaDongEntities Db = new HoaDonHaDongEntities();
            if (TienNopTheoThangID != null && TienNopTheoThangID.Length > 0 && soTien != null && soTien.Length > 0)
            {
                Double t = Convert.ToDouble(soTien);
                int tienThu = Convert.ToInt32(t);
                int tienNopTheoThangID = Convert.ToInt32(TienNopTheoThangID);
                //tìm record số tiền phải nộp theo tháng
                SoTienNopTheoThang Existed_Emp = Db.SoTienNopTheoThangs.Find(Convert.ToInt32(TienNopTheoThangID));
                Boolean isEdited = Existed_Emp.SoTienDaThu != 0 && Existed_Emp.SoTienDaThu != null;
                int tienThuEdit = 0;
                //nếu số tiền đã thu đã tồn tại từ trước, lưu lại giá trị của số tiền đã thu trước khi dc chỉnh sửa
                if (isEdited)
                {
                    tienThuEdit = (int)Existed_Emp.SoTienDaThu;
                }
                //lưu lại số tiền đã thu
                Existed_Emp.SoTienDaThu = tienThu;
                Db.SaveChanges();
                //kiểm tra xem dư có đã tồn tại hay chưa
                int count = Db.DuCoes.Where(c => c.TienNopTheoThangID == tienNopTheoThangID).Count();
                Boolean recordExisted = false;
                if (count > 0)
                {
                    recordExisted = true;
                }
                //nếu nhập dữ liệu lần đầu
                if (!isEdited)
                {
                    //nếu khách hàng đóng dư từ tháng trước(mặc định số tiền dư ban đầu của tháng này bằng số tiền dư tháng trước)
                    if (recordExisted)
                    {
                        DuCo duco = Db.DuCoes.Where(x => x.TienNopTheoThangID == Existed_Emp.ID).FirstOrDefault();
                        //trường họp nộp tiền xong vẫn tạo dư, update lại record dư có của tháng này
                        if (Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop + duco.SoTienDu > 0)
                        {
                            duco.SoTienDu = Convert.ToInt32(Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop + duco.SoTienDu);
                        }
                        //hết dư, set về 0
                        else if (Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop + duco.SoTienDu == 0)
                        {
                            duco.SoTienDu = 0;
                        }
                    }
                    //nếu dư mới xuất hiện ở tháng này
                    else
                    {
                        //tạo record mới
                        if (Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop > 0)
                        {
                            DuCo newduco = new DuCo();
                            newduco.KhachhangID = Existed_Emp.Hoadonnuoc.KhachhangID;
                            newduco.TienNopTheoThangID = tienNopTheoThangID;
                            newduco.SoTienDu = Convert.ToInt32(Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop);
                            newduco.KhachhangID = Existed_Emp.Hoadonnuoc.KhachhangID;
                            Db.DuCoes.Add(newduco);
                        }
                    }
                }
                //trường hợp thay đổi số tiền đã thu do lý do chủ quan
                else
                {
                    //nếu có dư từ tháng trước
                    if (recordExisted)
                    {
                        DuCo duco = Db.DuCoes.Where(x => x.TienNopTheoThangID == Existed_Emp.ID).FirstOrDefault();
                        //tính toán lại số tiền dư = số dư tháng này + (dư trước khi update - số dư bị nhập sai)
                        int duMoi = (int)Existed_Emp.SoTienDaThu - (int)Existed_Emp.SoTienPhaiNop;
                        int duCu = tienThuEdit - (int)Existed_Emp.SoTienPhaiNop;
                        var tt = Existed_Emp.Hoadonnuoc.Trangthaithu;
                        int tienDuUpdated = (int)Existed_Emp.SoTienDaThu - (int)Existed_Emp.SoTienPhaiNop + (int)duco.SoTienDu - (tienThuEdit - (int)Existed_Emp.SoTienPhaiNop);
                        if (tt == true)
                        {
                            if (tienDuUpdated > 0)
                            {
                                duco.SoTienDu = tienDuUpdated;

                            }
                            else if (tienDuUpdated == 0)
                            {
                                duco.SoTienDu = 0;
                            }
                            else if (tienDuUpdated < 0)
                            {
                                duco.SoTienDu = Convert.ToInt32(duco.SoTienDu - (tienThuEdit - Existed_Emp.SoTienPhaiNop));
                            }
                        }
                        else
                        {
                            duco.SoTienDu = (int)duco.SoTienDu - (tienThuEdit - (int)Existed_Emp.SoTienPhaiNop);
                        }
                    }
                    else
                    {
                        if (Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop > 0)
                        {
                            DuCo newduco = new DuCo();
                            newduco.KhachhangID = Existed_Emp.Hoadonnuoc.KhachhangID;
                            newduco.TienNopTheoThangID = tienNopTheoThangID;
                            newduco.SoTienDu = Convert.ToInt32(Existed_Emp.SoTienDaThu - Existed_Emp.SoTienPhaiNop);
                            Db.DuCoes.Add(newduco);
                        }
                    }
                }
                Db.SaveChanges();
            }
            if (action == "Index")
                return Redirect("Index");
            else
                return Redirect("congNoDoanhNghiep");

        }


        [HttpPost]
        public void changeGiaDich(String tiennoptheothangID, String sotien)
        {
            int tien = Convert.ToInt32(sotien);
            int id = Convert.ToInt32(tiennoptheothangID);
            var tienoptheothang = db.SoTienNopTheoThangs.Find(id);
            var giaodich = db.GiaoDiches.Where(p => p.TienNopTheoThangID == id).OrderBy(p => p.GiaoDichID).ToList().Last();
            int tiencu = giaodich.SoTien.Value;
            var duco = db.DuCoes.Where(p => p.TienNopTheoThangID == id).FirstOrDefault();
            //update số tiền đã thu ở 2 bảng số tiền nộp theo tháng và giao dịch
            giaodich.SoTien = tien;
            tienoptheothang.SoTienDaThu = tienoptheothang.SoTienDaThu - tiencu + tien;
            if (duco != null)
            {
                giaodich.SoDu = giaodich.SoDu - tiencu + tien;
                if ((duco.SoTienDu - tiencu + tien) > 0)
                {
                    duco.SoTienDu = duco.SoTienDu - tiencu + tien;
                }
                else if ((duco.SoTienDu - tiencu + tien) == 0)
                {
                    giaodich.SoDu = 0;
                    db.DuCoes.Attach(duco);
                    db.DuCoes.Remove(duco);
                }
                else
                {
                    giaodich.SoDu = 0;
                }
            }
            else
            {
                giaodich.SoDu = Convert.ToInt32(tienoptheothang.SoTienDaThu - tienoptheothang.SoTienPhaiNop);
                if ((tienoptheothang.SoTienDaThu - tienoptheothang.SoTienPhaiNop) > 0)
                {
                    var dumoi = new DuCo();
                    dumoi.TienNopTheoThangID = id;
                    dumoi.KhachhangID = tienoptheothang.Hoadonnuoc.KhachhangID;

                    dumoi.SoTienDu = Convert.ToInt32(tienoptheothang.SoTienDaThu - tienoptheothang.SoTienPhaiNop);
                    db.DuCoes.Add(dumoi);
                }
            }
            db.SaveChanges();
        }

        // GET: /Hoadon/congNoDoanhNghiep
        // mặc định khách hàng có ID là 2 sẽ là tập thể, saonh ngiệp
        public ActionResult congNoDoanhNghiep()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            var hoadonnuocs = db.Hoadonnuocs.Take(0);


            //lọc 
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false && p.IsDelete == true).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }

        //tương tự hàm Index sử dụng post request
        // mặc định khách hàng có ID là 2 sẽ là tập thể, saonh ngiệp
        //hàm trả về giá trị số tiền phải nộp theo tháng và TienNopTheoThangID để update vào form trong view
        [HttpPost]
        public ActionResult congNoDoanhNghiep(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            Boolean tim = false;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.LoaiKHID != KhachHang.SINHHOAT && p.Khachhang.TuyenKHID == tuyenID && p.Trangthaiin == true
               && (p.ThangHoaDon.Value == now.Month && p.NamHoaDon == now.Year || p.Trangthaithu == false || p.Trangthaithu == null)
               ).OrderBy(p => p.Khachhang.TTDoc);
            // nếu tuyến chưa được chọn, lấy hóa đơn theo form địa chỉ, tên, mã
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) &&
                        p.Khachhang.LoaiKHID != KhachHang.SINHHOAT &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) ||
                        (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.LoaiKHID != KhachHang.SINHHOAT
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                           p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.LoaiKHID != KhachHang.SINHHOAT
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) &&
                            p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).OrderBy(p => p.Ten).ToList();
            ViewBag.selected = tuyenID;
            ViewBag.tim = tim;
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            if (tim == true)
            {
                return View(hoadonnuocs.Take(200).ToList());
            }
            return View(hoadonnuocs.ToList());
        }

        //hiển thị khách hàng thanh toán bằng chuyển khoản, mặc định hình thức thanh toán là 2
        //hàm trả về giá trị số tiền phải nộp theo tháng và TienNopTheoThangID để update vào form trong view
        public ActionResult congNoChuyenKhoan()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            var hoadonnuocs = db.Hoadonnuocs.Take(0);


            //lọc 
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false && p.IsDelete == true).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }

        //tương tự hàm Index sử dụng post request
        [HttpPost]
        public ActionResult congNoChuyenKhoan(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);

            Boolean tim = false;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            var hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.HinhthucttID == 2 && p.Khachhang.TuyenKHID == tuyenID && p.Trangthaiin == true
                && (p.ThangHoaDon.Value == now.Month && p.NamHoaDon == now.Year || p.Trangthaithu == false || p.Trangthaithu == null)
                ).OrderBy(p => p.Khachhang.TTDoc);
            // nếu tuyến chưa được chọn, lấy hóa đơn theo form địa chỉ, tên, mã
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.HinhthucttID == 2 &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) || (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.HinhthucttID == 2
                            && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                           p.Khachhang.Diachi.ToLower().Contains(diachi) && p.Khachhang.HinhthucttID == 2
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) && p.Khachhang.HinhthucttID == 2 &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))
                            && p.Khachhang.HinhthucttID == 2).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            ViewBag.tim = tim;
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            var stto = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == stto).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            return View(hoadonnuocs.ToList());
        }

        public ActionResult KhachHangChuaNopTien()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            var hoadonnuocs = db.Hoadonnuocs.Take(0);
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            filterViewBagList();
            return View(hoadonnuocs.ToList());
        }

        [HttpPost]
        public ActionResult KhachHangChuaNopTien(FormCollection form)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            bool? loai = null;
            if (Request.QueryString["loaiKH"] != null)
            {
                loai = Request.QueryString["loaiKH"] != "1";
            }
            Boolean tim = false;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var prevmonth = now.Month - 1;
            var prevyear = now.Year;
            if (now.Month == 1)
            {
                prevmonth = 12;
                prevyear--;
            }
            int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            var hoadonnuocs = db.Hoadonnuocs.Where(p => (p.Khachhang.LoaiKHID == 1) != loai && p.Khachhang.TuyenKHID == tuyenID && p.Trangthaiin == true
                && (p.ThangHoaDon.Value == now.Month && p.NamHoaDon == now.Year && (p.Trangthaithu == false || p.Trangthaithu == null))
                ).OrderBy(p => p.Khachhang.TTDoc);
            if (!String.IsNullOrEmpty(form["diachi"]) || !String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["Makh"]))
            {
                tim = true;
                //địa chỉ dc nhập
                if (!String.IsNullOrEmpty(form["diachi"]))
                {
                    var diachi = form["diachi"].ToLower();
                    hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.Diachi.ToLower().Contains(diachi) && (p.Khachhang.LoaiKHID == 1) != loai &&
                        ((p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear) || (p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    int co = hoadonnuocs.Count();
                }
                //tên dc nhập
                if (!String.IsNullOrEmpty(form["name"]))
                {
                    var TenKH = form["name"].ToLower();
                    //ô địa chỉ có value
                    if (String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                            ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                            (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) &&
                             p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    //ng dùng nhập cả địa chỉ và tên khách hàng
                    else
                    {
                        var diachi = form["diachi"].ToLower();
                        hoadonnuocs = db.Hoadonnuocs.Where(p =>
                           ((p.NamHoaDon == now.Year && p.ThangHoaDon == now.Month) ||
                           (p.NamHoaDon == prevyear && p.ThangHoaDon == prevmonth)) && p.Khachhang.Diachi.ToLower().Contains(diachi)
                           && p.Khachhang.Ten.ToLower().Contains(TenKH)).OrderBy(p => p.Khachhang.TTDoc).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    int l2 = hoadonnuocs.Count();
                }
                //tìm theo mã khách hàng
                if (!String.IsNullOrEmpty(form["Makh"]))
                {
                    var maKH = form["Makh"].ToLower();
                    if (!String.IsNullOrEmpty(form["name"]) || !String.IsNullOrEmpty(form["diachi"]))
                    {
                        hoadonnuocs = hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToString().ToLower().Contains(maKH) &&
                            p.Khachhang.LoaiKHID == KhachHang.SINHHOAT &&
                            ((p.ThangHoaDon == prevmonth || p.ThangHoaDon == now.Month) &&
                            (p.NamHoaDon == prevyear || p.NamHoaDon == now.Year))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                    else
                    {

                        hoadonnuocs = db.Hoadonnuocs.Where(p => p.Khachhang.MaKhachHang.ToLower().Contains(maKH) &&
                            ((p.ThangHoaDon == now.Month && p.NamHoaDon == now.Year) ||
                            (p.ThangHoaDon == prevmonth && p.NamHoaDon == prevyear))).OrderBy(p => p.Khachhang.TTDoc);
                    }
                }
            }
            ViewBag.tim = tim;
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;
            ViewBag.selectedNhanvien = form["nhanvien"];
            ViewBag.SelectedTuyen = tuyenID;
            ViewBag.selectedTo = db.Nguoidungs.Find(Convert.ToInt32(Session["nguoiDungID"])).Nhanvien.ToQuanHuyenID;
            filterViewBagList();
            return View(hoadonnuocs.ToList());
        }

        //báo cáo  nợ theo tháng
        //mặc định tháng, năm hiện tại, sao khi chọn tháng từ form, dữ get request dc gửi dến controller
        public ActionResult baoCaoCongNo()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;
            var nhanvien = 0;
            if (Request.QueryString["nhanvienid"] != null && Request.QueryString["nhanvienid"].ToString().Length > 0)
            {
                nhanvien = Convert.ToInt32(Request.QueryString["nhanvienid"]);
            }
            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString() != "")
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["month"].ToString() != "")
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }

            var hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.NamHoaDon == year && (h.Trangthaithu == null || h.Trangthaithu == false) && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);
            //nếu người dùng có chọn 1 nhân viên
            if (nhanvien != 0)
            {
                hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.NamHoaDon == year && (h.Trangthaithu == null || h.Trangthaithu == false) && h.NhanvienID == nhanvien);
            }
            return View(hoadonnuocs.ToList());
        }



        //báo cáo khách hàng đóng tiền dư theo tháng
        //mặc định tháng, năm hiện tại, sao khi chọn tháng từ form, dữ get request dc gửi dến controller
        public ActionResult BaoCaoDuNo()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;
            var nhanvien = 0;
            if (Request.QueryString["nhanvienid"] != null && Request.QueryString["nhanvienid"].ToString().Length > 0)
            {
                nhanvien = Convert.ToInt32(Request.QueryString["nhanvienid"]);
            }
            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString() != "")
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["year"].ToString() != "")
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }
            var Duco = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh && h.Khachhang.LoaiKHID != KhachHang.SINHHOAT);
            //nếu người dùng có chọn 1 nhân viên
            if (nhanvien != 0)
            {
                Duco = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year && h.SoTienNopTheoThang.Hoadonnuoc.NhanvienID == nhanvien && h.Khachhang.LoaiKHID != KhachHang.SINHHOAT);
            }
            return View(Duco.ToList());
        }

        //Danh sách Khách hàng in hóa đơn
        public ActionResult DanhSachKhachHangInHoaDon()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;

            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString() != "")
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["month"].ToString() != "")
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }
            var hoadon = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && h.Khachhang.LoaiKHID != KhachHang.SINHHOAT && h.Khachhang.TuyenKHID == (int)selectedChiNhanh).ToList();
            if (Request.QueryString["tuyen"] != null && Request.QueryString["tuyen"].ToString() != "")
            {
                var tuyen = Convert.ToInt32(Request.QueryString["tuyen"]);
                hoadon = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && h.Khachhang.LoaiKHID != KhachHang.SINHHOAT && h.Khachhang.TuyenKHID == tuyen && h.Khachhang.TuyenKHID == (int)selectedChiNhanh).ToList();
            }
            return View(hoadon);
        }

        // tạo file excel
        //hàm này k còn sử dụng nữa vì đã tạo file bằng java script rồi
        public ExcelPackage createFile(String thang, String nam, String action)
        {
            String name;
            if (action == "baoCaoCongNo")
                name = "doanh thu";
            else if (action == "thieuNo")
                name = "du no";
            else if (action == "inhoadon")
                name = "khách hàng in hóa đơn";
            else
                name = "du co";
            FileInfo newFile = new FileInfo(@"C:\Bao cao " + name + " thang " + thang + "-" + nam + ".xls");
            System.IO.File.Delete(@"C:\Bao cao " + name + " thang " + thang + "-" + nam + ".xls");
            ExcelPackage ep = new ExcelPackage(newFile);
            ep.Workbook.Properties.Author = "NCK";
            // Tạo title cho file Excel
            ep.Workbook.Properties.Title = "Bao cao thu ngan";
            // Add Sheet vào file Excel
            ep.Workbook.Worksheets.Add("Báo cáo tháng " + thang + "/" + nam);
            var workSheet = ep.Workbook.Worksheets[1];

            var intro1 = workSheet.Cells["B1:C1"];
            intro1.Merge = true;
            intro1.Value = "CTY TNHH MTV NƯỚC SẠCH HÀ ĐÔNG";
            intro1.Style.Font.Size = 14;
            var intro2 = workSheet.Cells["B2:C2"];
            intro2.Merge = true;
            intro2.Value = "CHI NHÁNH - XN THU NGÂN";
            intro2.Style.Font.Bold = true;
            intro2.Style.Font.UnderLine = true;
            intro2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            var intro3 = workSheet.Cells["D1:F1"];
            intro3.Merge = true;
            intro3.Style.Font.Bold = true;
            intro3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            intro3.Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
            var cre = workSheet.Cells["D2:F2"];
            cre.Merge = true;
            cre.Style.Font.Bold = true;
            cre.Style.Font.UnderLine = true;
            cre.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cre.Value = "Độc lập - Tự do - Hạnh phúc";
            var date = workSheet.Cells["D3:F3"];
            date.Merge = true;
            date.Style.Font.Italic = true;
            date.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            date.Value = "Hà Đông, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
            return ep;
        }

        //hàm xuất ra file excel
        //nhận tháng năm, tên action từ congno.js
        //hàm này k còn sử dụng nữa vì đã tạo file bằng java script rồi
        public ActionResult ExportFile(String thang, String nam, String action)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            String t = action;
            //tạo model
            var model = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien);
            var mod = db.DuCoes.AsQueryable();
            int month = Convert.ToInt32(thang);
            int year = Convert.ToInt32(nam);
            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;
            //report báo cáo doanh thu
            if (action == "baoCaoCongNo")
            {
                model = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && h.Trangthaithu == true && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);

                using (var excelPackage = createFile(thang, nam, action))
                {
                    // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                    var workSheet = excelPackage.Workbook.Worksheets[1];
                    workSheet.Cells.Style.Font.Name = "Times New Roman";
                    var intro4 = workSheet.Cells["B5:E5"];
                    intro4.Merge = true;
                    intro4.Value = "BẢNG KÊ KHÁCH HÀNG DƯ NỢ THÁNG " + thang + " NĂM " + nam;
                    intro4.Style.Font.Bold = true;
                    int i = 1;
                    workSheet.Cells[6, 1].Value = "TT";
                    workSheet.Cells[6, 2].Value = "Tên khách hàng";
                    workSheet.Cells[6, 3].Value = "MKH";
                    workSheet.Cells[6, 4].Value = "Địa chỉ";
                    workSheet.Cells[6, 5].Value = "Tháng";
                    workSheet.Cells[6, 6].Value = "Số tiền";
                    int tongTien = 0;
                    //đổ dữ liệu từ model vào
                    foreach (var item in model)
                    {
                        workSheet.Cells[i + 6, 1].Value = i;
                        workSheet.Cells[i + 6, 2].Value = item.Khachhang.Ten;
                        workSheet.Cells[i + 6, 3].Value = item.Khachhang.MaKhachHang;
                        workSheet.Cells[i + 6, 4].Value = item.Khachhang.Diachi;
                        workSheet.Cells[i + 6, 5].Value = item.ThangHoaDon.ToString();
                        workSheet.Cells[i + 6, 6].Value = item.SoTienNopTheoThang.SoTienDaThu;
                        if (item.SoTienNopTheoThang != null)
                            tongTien = tongTien + item.SoTienNopTheoThang.SoTienDaThu.Value;
                        i++;
                    }
                    int row = i + 6;
                    var tong = workSheet.Cells["A" + row + ":" + "E" + row];
                    tong.Merge = true;
                    tong.Value = "Tổng";
                    workSheet.Cells[row, 6].Value = tongTien;
                    for (int j = 6; j <= row; j++)
                    {
                        for (int g = 1; g <= 6; g++)
                        {
                            workSheet.Cells[j, g].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }
                    // BindingFormatForExcel(workSheet, list);
                    excelPackage.Save();
                    String dir = excelPackage.File.DirectoryName.ToString() + "/" + excelPackage.File.Name;
                    ViewBag.dir = dir;
                    System.Diagnostics.Process.Start(dir);
                }
            }
            else if (action == "thieuNo")
            {
                model = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && (h.Trangthaithu == false || h.Trangthaithu == null) && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);
                using (var excelPackage = createFile(thang, nam, action))
                {

                    var workSheet = excelPackage.Workbook.Worksheets[1];
                    workSheet.Cells.Style.Font.Name = "Times New Roman";
                    var intro4 = workSheet.Cells["B5:E5"];
                    intro4.Merge = true;
                    intro4.Value = "BẢNG KÊ KHÁCH HÀNG DƯ NỢ THÁNG " + thang + " NĂM " + nam;
                    intro4.Style.Font.Bold = true;
                    int i = 1;
                    workSheet.Cells[6, 1].Value = "TT";
                    workSheet.Cells[6, 2].Value = "Tên khách hàng";
                    workSheet.Cells[6, 3].Value = "MKH";
                    workSheet.Cells[6, 4].Value = "Địa chỉ";
                    workSheet.Cells[6, 5].Value = "Tháng";
                    workSheet.Cells[6, 6].Value = "Số tiền";
                    double tongTien = 0;
                    //đổ dữ liệu từ model vào file
                    foreach (var item in model)
                    {
                        workSheet.Cells[i + 6, 1].Value = i;
                        workSheet.Cells[i + 6, 2].Value = item.Khachhang.Ten;
                        workSheet.Cells[i + 6, 3].Value = item.Khachhang.MaKhachHang;
                        workSheet.Cells[i + 6, 4].Value = item.Khachhang.Diachi;
                        workSheet.Cells[i + 6, 5].Value = item.ThangHoaDon.ToString();
                        if (item.SoTienNopTheoThang != (Object)null)
                        {
                            workSheet.Cells[i + 6, 6].Value = item.SoTienNopTheoThang.SoTienPhaiNop;
                        }
                        else workSheet.Cells[i + 6, 6].Value = 0;
                        if (item.SoTienNopTheoThang != null)
                            tongTien = tongTien + item.SoTienNopTheoThang.SoTienPhaiNop.Value;
                        i++;
                    }
                    int row = i + 6;
                    var tong = workSheet.Cells["A" + row + ":" + "E" + row];
                    tong.Merge = true;
                    tong.Value = "Tổng";
                    workSheet.Cells[row, 6].Value = tongTien;
                    //tạo border
                    for (int j = 6; j <= row; j++)
                    {
                        for (int g = 1; g <= 6; g++)
                        {
                            workSheet.Cells[j, g].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }
                    excelPackage.Save();
                    String dir = excelPackage.File.DirectoryName.ToString() + "/" + excelPackage.File.Name;
                    ViewBag.dir = dir;
                    System.Diagnostics.Process.Start(dir);
                }
            }
            else if (action == "duNo")
            {

                mod = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);
                using (var excelPackage = createFile(thang, nam, action))
                {
                    var workSheet = excelPackage.Workbook.Worksheets[1];
                    workSheet.Cells.Style.Font.Name = "Times New Roman";
                    var intro4 = workSheet.Cells["B5:E5"];
                    intro4.Merge = true;
                    intro4.Value = "BẢNG KÊ KHÁCH HÀNG DƯ CÓ THÁNG " + thang + " NĂM " + nam;
                    intro4.Style.Font.Bold = true;
                    int i = 1;
                    workSheet.Cells[6, 1].Value = "TT";
                    workSheet.Cells[6, 2].Value = "Tên khách hàng";
                    workSheet.Cells[6, 3].Value = "MKH";
                    workSheet.Cells[6, 4].Value = "Địa chỉ";
                    workSheet.Cells[6, 5].Value = "Tháng";
                    workSheet.Cells[6, 6].Value = "Số tiền";
                    double tongTien = 0;
                    foreach (var item in mod)
                    {
                        workSheet.Cells[i + 6, 1].Value = i;
                        workSheet.Cells[i + 6, 2].Value = item.Khachhang.Ten;
                        workSheet.Cells[i + 6, 3].Value = item.Khachhang.MaKhachHang;
                        workSheet.Cells[i + 6, 4].Value = item.Khachhang.Diachi;
                        workSheet.Cells[i + 6, 5].Value = item.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon;
                        workSheet.Cells[i + 6, 6].Value = item.SoTienDu;
                        if (item.SoTienNopTheoThang != null)
                            tongTien = tongTien + item.SoTienDu.Value;
                        i++;
                    }
                    int row = i + 6;
                    var tong = workSheet.Cells["A" + row + ":" + "E" + row];
                    tong.Merge = true;
                    tong.Value = "Tổng";
                    workSheet.Cells[row, 6].Value = tongTien;

                    for (int j = 6; j <= row; j++)
                    {
                        for (int g = 1; g <= 6; g++)
                        {
                            workSheet.Cells[j, g].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }
                    excelPackage.Save();
                    String dir = excelPackage.File.DirectoryName.ToString() + "/" + excelPackage.File.Name;
                    ViewBag.dir = dir;
                    System.Diagnostics.Process.Start(dir);
                }
            }
            else
            {
                model = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year);
                using (var excelPackage = createFile(thang, nam, action))
                {
                    var workSheet = excelPackage.Workbook.Worksheets[1];
                    workSheet.Cells.Style.Font.Name = "Times New Roman";
                    var intro4 = workSheet.Cells["B5:E5"];
                    intro4.Merge = true;
                    intro4.Value = "DANH SÁCH KHÁCH HÀNG IN HÓA ĐƠN THÁNG " + thang + " NĂM " + nam;
                    intro4.Style.Font.Bold = true;
                    int i = 1;
                    workSheet.Cells[6, 1].Value = "TT";
                    workSheet.Cells[6, 2].Value = "Tên khách hàng";
                    workSheet.Cells[6, 3].Value = "MKH";
                    workSheet.Cells[6, 4].Value = "Địa chỉ";
                    workSheet.Cells[6, 5].Value = "Sản lượng";
                    workSheet.Cells[6, 6].Value = "Số tiền";
                    var tongTien = 0;
                    foreach (var item in model)
                    {
                        workSheet.Cells[i + 6, 1].Value = i;
                        workSheet.Cells[i + 6, 2].Value = item.Khachhang.Ten;
                        workSheet.Cells[i + 6, 3].Value = item.Khachhang.MaKhachHang;
                        workSheet.Cells[i + 6, 4].Value = item.Khachhang.Diachi;
                        workSheet.Cells[i + 6, 5].Value = item.Tongsotieuthu;
                        workSheet.Cells[i + 6, 6].Value = item.SoTienNopTheoThang.SoTienPhaiNop;
                        if (item.SoTienNopTheoThang.SoTienPhaiNop != null)

                            tongTien = Convert.ToInt32(tongTien + item.SoTienNopTheoThang.SoTienPhaiNop);
                        i++;
                    }
                    int row = i + 6;
                    var tong = workSheet.Cells["A" + row + ":" + "E" + row];
                    tong.Merge = true;
                    tong.Value = "Tổng";
                    workSheet.Cells[row, 6].Value = tongTien;

                    for (int j = 6; j <= row; j++)
                    {
                        for (int g = 1; g <= 6; g++)
                        {
                            workSheet.Cells[j, g].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }

                    excelPackage.Save();
                    String dir = excelPackage.File.DirectoryName.ToString() + "/" + excelPackage.File.Name;
                    ViewBag.dir = dir;
                    System.Diagnostics.Process.Start(dir);
                }
            }

            return View("ExportFile", (Object)t);
        }

        //trang dẫn tới các báo cáo thu ngân
        public ActionResult ReportList()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            int phongBanID = Convert.ToInt32(Session["phongBan"]); ;

            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;
            var tuyen = db.Tuyenkhachhangs.ToList();
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            ViewBag.selectedTenPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), chinhanh, 1); ;
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == 0).ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.OrderBy(p => p.Ten).ToList();
            int toNV = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            ViewBag.to = db.ToQuanHuyens.Where(p => p.QuanHuyenID == toNV && p.PhongbanID == phongBanID).ToList();
            ViewBag.rl = "rl";
            return View(tuyen);
        }

        public ActionResult DanhDauTatCa()
        {
            int selectedChiNhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            String isChecked = "1";

            var checklist = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/isChecked.txt")).ToList();
            var found = false;
            foreach (String ck in checklist)
            {
                String[] br = ck.Split(' ');
                if (br.Count() > 1 && br[1] == selectedChiNhanh.ToString())
                {
                    isChecked = ck;
                    found = true;
                }
            }
            Console.WriteLine(found);
            Console.WriteLine(isChecked);

            if (found)
            {
                String[] ch = isChecked.Split(' ');
                isChecked = ch[2];
            }
            else
            {
                System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), DateTime.Now.Month + " " + selectedChiNhanh + " 1" + Environment.NewLine);
            }
            Console.WriteLine(isChecked);
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            int dem = db.Hoadonnuocs.Where(h => h.Khachhang.QuanhuyenID == selectedChiNhanh && h.ThangHoaDon == now.Month && h.NamHoaDon == now.Year).Count();
            ViewBag.isChecked = isChecked;
            ViewBag.dem = dem;
            return View();
        }

        public String CheckAll()
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            var r = System.IO.File.ReadLines(Server.MapPath(@"~/Controllers/isChecked.txt")).ToList();
            String finded = "";
            for (int i = 0; i < r.Count(); i++)
            {
                String[] line = r[i].Split(' ');
                if (line.Count() > 1 && line[1] == chinhanh.ToString())
                {
                    finded = Environment.NewLine + DateTime.Now.Month + " " + chinhanh + " 0";
                    r[i] = finded;
                }

            }
            System.IO.File.WriteAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), "");
            foreach (String s in r)
            {
                if (s.Length > 1)
                {
                    System.IO.File.AppendAllText(Server.MapPath(@"~/Controllers/isChecked.txt"), s + Environment.NewLine);
                }
            }


            var now = DateTime.Now;
            String sql = "UPDATE [HoaDonHaDong].[dbo].[Hoadonnuoc] SET Trangthaithu = 1, NgayNopTien = CAST(GETDATE() AS DATE) from Hoadonnuoc join [HoaDonHaDong].[dbo].Khachhang on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID  WHERE Hoadonnuoc.ThangHoaDon =" + now.Month + " and Hoadonnuoc.NamHoaDon = " + now.Year + "  and Khachhang.QuanhuyenID =" + chinhanh + ";";
            var updateCmd = db.Database.ExecuteSqlCommand(sql);
            String sql2 = "UPDATE [HoaDonHaDong].[dbo].SoTienNopTheoThang SET SoTienDaThu = SoTienPhaiNop from SoTienNopTheoThang join [HoaDonHaDong].[dbo].Hoadonnuoc on SoTienNopTheoThang.HoaDonNuocID = Hoadonnuoc.HoadonnuocID join [HoaDonHaDong].[dbo].Khachhang on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID  WHERE Hoadonnuoc.ThangHoaDon =" + now.Month + " and Hoadonnuoc.NamHoaDon = " + now.Year + " and Khachhang.QuanhuyenID =" + chinhanh + " ;";
            var updateSotien = db.Database.ExecuteSqlCommand(sql2);
            var hoadonnuocs = (from c1 in db.Hoadonnuocs
                               join c2 in db.DuCoes on c1.SoTienNopTheoThang.ID equals c2.TienNopTheoThangID
                               where c1.Khachhang.QuanhuyenID == chinhanh && (c1.Trangthaithu != true || c1.Trangthaithu == null)
                               && (c1.ThangHoaDon.Value == now.Month && c1.NamHoaDon == now.Year)
                               select new Models.HoaDonDayDu { h = c1, d = c2 });
            hoadonnuocs.ToList().ForEach(hd =>
                {
                    if (hd.d.SoTienDu > hd.h.SoTienNopTheoThang.SoTienPhaiNop)
                    { hd.d.SoTienDu = hd.d.SoTienDu - Convert.ToInt32(hd.h.SoTienNopTheoThang.SoTienPhaiNop); }
                });
            var sql3 = "INSERT INTO [dbo].[GiaoDich]([TienNopTheoThangID],[NgayGiaoDich],[SoTien],[SoDu]) SELECT  [ID] ,CAST(GETDATE() AS DATE),[SoTienPhaiNop],SoTienDu FROM [HoaDonHaDong].[dbo].[SoTienNopTheoThang] join [HoaDonHaDong].[dbo].[Hoadonnuoc] on Hoadonnuoc.SoTienNopTheoThangID = SoTienNopTheoThang.id left join [HoaDonHaDong].[dbo].[DuCo] on SoTienNopTheoThang.ID = DuCo.TienNopTheoThangID join [HoaDonHaDong].[dbo].[Khachhang] on Hoadonnuoc.KhachhangID = Khachhang.KhachhangID WHERE  ThangHoaDon =" + now.Month + " and NamHoaDon = " + now.Year + "  and Khachhang.QuanhuyenID = " + chinhanh + ";";
            var themgiaodich = db.Database.ExecuteSqlCommand(sql3);
            var updateNull = db.Database.ExecuteSqlCommand("UPDATE [dbo].[GiaoDich] SET [SoDu] = 0 WHERE [SoDu] is null");
            db.SaveChanges();
            return "";
        }

        //hàm trả về thông tin các giao dịch của khách hàng
        //khi khách hàng nhấn vào nút dấu cộng ở số tiền đã nộp
        [HttpPost]
        public JsonResult getGiaoDich(String khID)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            int KHID = Convert.ToInt32(khID);
            var now = DateTime.Now;
            var hoadonnuocs = (from c1 in db.Hoadonnuocs
                               join c2 in db.GiaoDiches on c1.SoTienNopTheoThang.ID equals c2.TienNopTheoThangID
                               where (c1.NamHoaDon == now.Year) && c1.KhachhangID == KHID
                               orderby c2.GiaoDichID descending
                               select new { ngay = c2.NgayGiaoDich.Value.Day.ToString()+"/"+c2.NgayGiaoDich.Value.Month.ToString()+"/"+c2.NgayGiaoDich.Value.Year.ToString(),
                                   sotien = c2.SoTien, du = c2.SoDu }).ToList();
            int dem = hoadonnuocs.Count();
            var result = Json(hoadonnuocs, JsonRequestBehavior.AllowGet);
            return result;
        }

        //thêm 1 giao dịch
        [HttpPost]
        public ActionResult addGiaoDich(String tiennoptheothangID, String sotien, String ngaynop)
        {
            if (!String.IsNullOrEmpty(tiennoptheothangID) && !String.IsNullOrEmpty(sotien))
            {
                var nd = ngaynop;
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("vi-VN");
                var ngayNop = DateTime.Now.Date;
                if (!String.IsNullOrEmpty(ngaynop))
                {
                    ngayNop = DateTime.ParseExact(ngaynop, "dd/MM/yyyy", culture);
                }

                var gd = new GiaoDich();
                gd.NgayGiaoDich = ngayNop;
                int tnttID = Convert.ToInt32(tiennoptheothangID);
                gd.TienNopTheoThangID = Convert.ToInt32(tiennoptheothangID);
                //update số tiền trong giao dich tìm dc
                gd.SoTien = Convert.ToInt32(sotien);
                var tiennoptheothang = db.SoTienNopTheoThangs.Find(tnttID);
                if (tiennoptheothang != null)
                {
                    //nếu khách hàng chưa nộp tiền
                    if (tiennoptheothang.SoTienDaThu == null)
                    {
                        tiennoptheothang.SoTienDaThu = Convert.ToInt32(sotien);
                        tiennoptheothang.Hoadonnuoc.Trangthaithu = true;
                    }
                    //khách hàng đã nộp tiền, update số tiền đã nộp cộng thêm số đóng thêm của giao dịch
                    else
                    {
                        tiennoptheothang.SoTienDaThu = tiennoptheothang.SoTienDaThu + Convert.ToInt32(sotien);
                    }
                }

                db.GiaoDiches.Add(gd);
                var duco = db.DuCoes.Where(d => d.TienNopTheoThangID == tnttID).FirstOrDefault();
                //kiểm tra xem đã có dư tồn tại từ trước chưa
                //nếu đã có dư từ tháng trc và khách hàng đã nộp tiền, cộng thêm dư có
                if (duco != null && tiennoptheothang.SoTienDaThu != null)
                {
                    duco.SoTienDu = duco.SoTienDu + Convert.ToInt32(sotien);
                    gd.SoDu = duco.SoTienDu;
                }
                //nếu chưa có dư có tháng trước và tiền nộp tạo dư, thêm record dư có
                else if (duco == null && (tiennoptheothang.SoTienDaThu + gd.SoTien) > tiennoptheothang.SoTienPhaiNop)
                {
                    int khID = tiennoptheothang.Hoadonnuoc.KhachhangID.Value;
                    duco = new DuCo();
                    duco.TienNopTheoThangID = tnttID;
                    duco.KhachhangID = khID;
                    duco.SoTienDu = Convert.ToInt32(tiennoptheothang.SoTienDaThu - tiennoptheothang.SoTienPhaiNop);
                    gd.SoDu = Convert.ToInt32(tiennoptheothang.SoTienDaThu - tiennoptheothang.SoTienPhaiNop);
                    db.DuCoes.Add(duco);
                }
                var hoadon = db.Hoadonnuocs.Where(h => h.SoTienNopTheoThangID == tnttID).FirstOrDefault();
                if (hoadon.Trangthaithu == false || hoadon.Trangthaithu == null)
                {
                    hoadon.Trangthaithu = true;
                    hoadon.NgayNopTien = ngayNop;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Congno");
        }

        public ActionResult DanhSachTuyenHoaDonDaIn()
        {
            ViewBag.beforeFiltered = true;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewData["tuyen"] = new List<Tuyenkhachhang>();
            return View();
        }

        [HttpPost]
        public ActionResult DanhSachTuyenHoaDonDaIn(FormCollection form)
        {
            //một tuyến được nhập xong chỉ số tức là tất cả hóa đơn trong đó đã nhập xong
            int month = String.IsNullOrEmpty(form["thang"]) ? DateTime.Now.Month : Convert.ToInt32(form["thang"]);
            int year = String.IsNullOrEmpty(form["year"]) ? DateTime.Now.Year : Convert.ToInt32(form["year"]);

            List<Tuyenkhachhang> newLs = new List<Tuyenkhachhang>();
            List<TuyenDuocChot> tuyen = db.TuyenDuocChots.Where(p => p.Nam == year && p.Thang == month).ToList();
            foreach (var item in tuyen)
            {
                Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(item.TuyenKHID);
                newLs.Add(tuyenKH);
            }

            ViewBag.beforeFiltered = false;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewBag.selectedMonth = month;
            ViewBag.selectedYear = year;
            ViewData["tuyen"] = newLs;
            return View();
        }

        public ActionResult XemChiTiet(String tuyen, String month, String year)
        {
            //Cập nhật trạng thái tính tiền
            int tuyenInt = Convert.ToInt32(tuyen);
            int monthInt = Convert.ToInt32(month);
            int yearInt = Convert.ToInt32(year);
            TuyenDuocChot chotTuyen = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenInt && p.Thang == monthInt && p.Nam == yearInt);
            if (chotTuyen != null)
            {
                chotTuyen.TrangThaiTinhTien = true;
                db.Entry(chotTuyen).State = EntityState.Modified;
                db.SaveChanges();
            }

            var danhSach = (from i in db.Lichsuhoadons
                            where i.TuyenKHID == tuyenInt && i.ThangHoaDon == monthInt && i.NamHoaDon == yearInt
                            select new
                            {
                                HoaDonNuoc = i.HoaDonID,
                                MaKH = i.MaKH,
                                TenKH = i.TenKH,
                                DiaChi = i.Diachi,
                                SH1 = i.SH1,
                                SH2 = i.SH2,
                                SH3 = i.SH3,
                                SH4 = i.SH4,
                                HC = i.HC,
                                CC = i.CC,
                                SX = i.SX,
                                KD = i.KD,
                                PhiVAT = i.ThueSuatPrice,
                                PhiBVMT = i.PhiBVMT,
                                TTDoc = i.TTDoc,
                                SanLuong = i.ChiSoMoi - i.ChiSoCu,
                                TongCong = i.TongCong
                            }).ToList();

            ViewBag.beforeFilter = false;
            ViewBag.dsachKH = danhSach.OrderBy(p => p.TTDoc).ToList();
            String tuyenID = Request.QueryString["tuyen"];
            return View();
        }


        // GET: /Hoadon/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Create
        public ActionResult Create()
        {
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan");
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten");
            return View();
        }

        // POST: /Hoadon/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HoadonnuocID,Ngayhoadown,KhachhangID,NhanvienID,Sohoadon,Kyhieu,Tongsotieuthu,Trangthaithu,Trangthaiin")] Hoadonnuoc hoadonnuoc)
        {
            if (ModelState.IsValid)
            {
                db.Hoadonnuocs.Add(hoadonnuoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // POST: /Hoadon/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HoadonnuocID,Ngayhoadown,KhachhangID,NhanvienID,Sohoadon,Kyhieu,Tongsotieuthu,Trangthaithu,Trangthaiin")] Hoadonnuoc hoadonnuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoadonnuoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            return View(hoadonnuoc);
        }

        // POST: /Hoadon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            db.Hoadonnuocs.Remove(hoadonnuoc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}