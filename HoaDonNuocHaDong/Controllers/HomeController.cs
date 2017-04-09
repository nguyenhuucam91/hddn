using HoaDonNuocHaDong.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Routing;
using HoaDonNuocHaDong.Base;

namespace HoaDonNuocHaDong.Controllers
{
    public class HomeController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        UserInfo info = new UserInfo();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //xóa không load từ cache ra
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnActionExecuting(filterContext);

            var countDB = db.Nguoidungs.Count();
            //set up tài khoản admin nếu hệ thống chưa có ai
            if (countDB == 0)
            {
                String firstHash = String.Concat(UserInfo.CreateMD5("123456").ToLower(), "123456");
                String md5MatKhau = UserInfo.CreateMD5(firstHash);

                Nguoidung _ngDung = new Nguoidung();
                _ngDung.Taikhoan = "admin";
                _ngDung.Matkhau = md5MatKhau.ToLower();
                _ngDung.Isadmin = true;
                db.Nguoidungs.Add(_ngDung);
                db.SaveChanges();

                Dangnhap _dangNhap = new Dangnhap();
                _dangNhap.NguoidungID = _ngDung.NguoidungID;
                _dangNhap.Solandangnhapsai = 0;
                db.Dangnhaps.Add(_dangNhap);
                db.SaveChanges();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Xử lí đăng nhập
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost, OutputCache(NoStore = true, Duration = 1)]

        public ActionResult Index(FormCollection form)
        {
            //Sesssion sử dụng cho việc giữ dữ  liệu của dropdownlist
            Session["selectedNhanvien"] = "";
            Session["solieuTieuThuNhanvien"] = "";

            String tenDangNhap = form["username"];
            String matKhau = form["password"];
            //mã hóa password 2 lần, chuyển về lowercase sau đó mã hóa lần 2
            String firstHash = String.Concat(UserInfo.CreateMD5(matKhau).ToLower(), matKhau);

            String md5MatKhau = UserInfo.CreateMD5(firstHash);

            // Response.Write(UserInfo.CreateMD5(UserInfo.CreateMD5("123456")+"123456")); Response.End();
            //kiểm tra DB, nếu empty thì quay lại trang đăng nhập, nếu > 0 thì 
            var countRecordDB = db.Nguoidungs.Count();
            TempData["message"] = null;
            if (countRecordDB > 0)
            {
                //kiểm tra username của người dùng, nếu không có trong db thì không cần check nữa
                var nguoiDung = db.Nguoidungs.Where(p => p.Taikhoan == tenDangNhap);
                if (nguoiDung.Count() > 0)
                {
                    //lấy mật khẩu trong DB ra và đối chiều vs mật khẩu ng dùng nhập vào, nếu thành công thì điều hướng về trang tương ứng về role
                    var passwordDB = nguoiDung.First().Matkhau;
                    //lưu session cho nguoiDungID và tên đăng nhập
                    int nguoiDungID = nguoiDung.First().NguoidungID;

                    if (passwordDB.ToLower() == md5MatKhau.ToLower())
                    {
                        //cập nhật lại state đăng nhập, reset số lần đăng nhập sai về 0
                        Dangnhap dangNhap = info.getDangNhap(nguoiDungID);
                        //nếu thời gian hiện tại > thời gian hết hạn khóa thì cho đăng nhập
                        if (dangNhap != null)
                        {
                            if (dangNhap.Thoigianhethankhoa == null)
                            {
                                dangNhap.Thoigiandangnhap = DateTime.Now;
                                dangNhap.Trangthaikhoa = false;
                                dangNhap.Solandangnhapsai = 0;
                                dangNhap.Thoigianhethankhoa = null;
                                //  db.Dangnhaps.Attach(dangNhap);
                                db.Entry(dangNhap).State = EntityState.Modified;
                                db.SaveChanges();
                                //lưu session liên quan
                                Session["nguoiDungID"] = nguoiDungID;
                                Session["tenDangNhap"] = tenDangNhap;
                                //tuyến người dùng hiện đang đăng nhập
                                var tuyenID = (from i in db.Tuyentheonhanviens
                                               join r in db.Nhanviens on i.NhanVienID equals r.NhanvienID
                                               join s in db.Nguoidungs on r.NhanvienID equals s.NhanvienID
                                               where s.NguoidungID == nguoiDungID
                                               select new
                                               {
                                                   TuyenKHID = i.TuyenKHID,
                                                   NhanVienID = i.NhanVienID
                                               }).ToList();
                                //nếu rỗng tuyến
                                if (tuyenID != null)
                                {
                                    String dsTuyen = "";
                                    foreach (var item in tuyenID)
                                    {
                                        dsTuyen = dsTuyen + item.TuyenKHID + ",";
                                    }
                                    //loại bỏ dấu , ở cuối chuỗi nếu ds tuyến ko rỗng
                                    if (!String.IsNullOrEmpty(dsTuyen))
                                    {
                                        dsTuyen = dsTuyen.Remove(dsTuyen.Length - 1);
                                    }
                                    Session["tuyenID"] = dsTuyen;

                                    var hasTuyenID = tuyenID.FirstOrDefault();
                                    if (hasTuyenID != null)
                                    {
                                        Session["nhanVienID"] = hasTuyenID.NhanVienID;
                                    }
                                }
                                bool checkAdmin = UserInfo.checkAdmin(nguoiDungID);
                                //lấy Phòng ban obj
                                if (!checkAdmin)
                                {
                                    Phongban phongBan = info.getPhongBan(nguoiDungID);
                                    if (phongBan != null)
                                    {
                                        Session["phongBan"] = phongBan.PhongbanID;
                                        List<int> kinhDoanhList = db.Phongbans.Where(p => p.Ten.Contains("kinh")).Select(p => p.PhongbanID).ToList();
                                        List<int> thuNganList = db.Phongbans.Where(p => p.Ten.Contains("thu")).Select(p => p.PhongbanID).ToList();
                                        List<int> inHoaDonList = db.Phongbans.Where(p => p.Ten.Contains("in")).Select(p => p.PhongbanID).ToList();
                                        //kinh doanh
                                        if (kinhDoanhList.Contains(phongBan.PhongbanID))
                                        {
                                            return RedirectToAction("index", "Khachhang");
                                        }
                                        //thu ngân
                                        else if (thuNganList.Contains(phongBan.PhongbanID))
                                        {
                                            return RedirectToAction("Index", "Congno");
                                        }
                                        //in hóa đơn
                                        else
                                        {
                                            return RedirectToAction("index", "Print");
                                        }
                                    }
                                }
                                //nếu là admin
                                else
                                {

                                    return RedirectToAction("Index", "Quanhuyen");
                                }
                            }
                        }

                    }

                    //nếu đăng nhập thất bại, tính số lần đăng nhập, nếu > 5 sẽ tiến hành khóa tài khoản trong khoảng thời gian nào đó, nếu nhập sai quá nhiều (bội của 5) thì cộng dồn
                    //thời gian hết hạn khóa.
                    else
                    {
                        Dangnhap dangNhap = info.getDangNhap(nguoiDungID);
                        int soLanDangNhapSai = UserInfo.getSoLanDangNhapSai(nguoiDungID);
                        //nếu số lần nhập lớn hơn SOLANDANGNHAPSAI thì khóa tài khoản, set ngày hết hạn = ngày bây h + 5

                        if (dangNhap != null)
                        {

                            if (soLanDangNhapSai >= UserInfo.SOLANDANGNHAPSAI)
                            {
                                //cập nhật thời gian đăng nhập  
                                dangNhap.Thoigiandangnhap = DateTime.Now;
                                dangNhap.Trangthaikhoa = true;

                                //cộng dồn thời gian nếu đăng nhập sai quá nhiều, nếu nhập sai 5 lần liên tiếp sẽ cộng dồn thời gian hết hạn lên
                                if (dangNhap.Solandangnhapsai % 5 == 0)
                                {
                                    if (dangNhap.Thoigianhethankhoa != null)
                                    {
                                        dangNhap.Thoigianhethankhoa = dangNhap.Thoigianhethankhoa.Value.AddDays(UserInfo.DATETHRESHOLD);
                                        //reset số lần đăng nhập sai về 0                                        
                                    }
                                    else
                                    {
                                        dangNhap.Thoigianhethankhoa = DateTime.Now.AddDays(UserInfo.DATETHRESHOLD);
                                    }
                                    dangNhap.Solandangnhapsai = 0;
                                }
                                //nếu không thì cứ tiếp tục cộng thêm số lần đăng nhập sai
                                else
                                {
                                    dangNhap.Solandangnhapsai = dangNhap.Solandangnhapsai + 1;
                                }
                                TempData["message"] = "Tài khoản của bạn đã bị khóa "+UserInfo.DATETHRESHOLD+" ngày do nhập sai password quá 5 lần. Xin hãy quay trở lại sau";

                            }
                            //nếu đã có nhưng chưa < số lần đăng nhập sai thì cập nhật thông tin đăng nhập, hiện thông báo kiểm tra lại mật khẩu
                            else
                            {
                                dangNhap.Thoigiandangnhap = DateTime.Now;
                                dangNhap.Solandangnhapsai = dangNhap.Solandangnhapsai + 1;
                                TempData["message"] = "Xin hãy kiểm tra lại mật khẩu";
                            }
                            //Attach và chuyển object state
                            //db.Dangnhaps.Attach(dangNhap);
                            db.Entry(dangNhap).State = EntityState.Modified;

                        }
                        //nếu ko có thì thêm mới
                        else
                        {
                            Dangnhap dangNhapThatBai = new Dangnhap();
                            dangNhapThatBai.NguoidungID = nguoiDungID;
                            dangNhapThatBai.Thoigiandangnhap = DateTime.Now;
                            dangNhapThatBai.Trangthaikhoa = false;
                            dangNhapThatBai.Solandangnhapsai = 1;
                            db.Dangnhaps.Add(dangNhapThatBai);
                            TempData["message"] = "Xin hãy kiểm tra lại mật khẩu";
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    TempData["message"] = "Không có thông tin người dùng trong hệ thống";
                }

            }

            return RedirectToAction("Index", "Home");
            //return null;
        }

        public ActionResult EditProfile()
        {
            //nếu không đăng nhập hệ thống thì tự động bắn về trang Login
            if (LoggedInUser.NguoidungID == 0)
            {
                return RedirectToAction("Index");
            }
            int nguoiDungID = LoggedInUser.NguoidungID;
            var nguoiDung = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
            //kiểm tra xem có assoc được không
            int? nhanVienID = nguoiDung.NhanvienID;
            if (nhanVienID != null)
            {
                //lấy thông tin nhân viên
                var nhanVien = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                ViewData["nhanVien"] = nhanVien;
            }
            else
            {
                ViewData["nhanVien"] = new Nhanvien();
            }
            ViewData["nguoiDung"] = nguoiDung;
            ViewBag.identicalPassword = true;
            //khu vực ViewData
            return View();
        }

        [HttpPost]
        public ActionResult EditProfile(FormCollection form)
        {
            //nếu không đăng nhập hệ thống thì tự động bắn về trang Login
            if (LoggedInUser.NguoidungID == 0)
            {
                return RedirectToAction("Index");
            }

            String password = form["password"];
            String reEnterPassword = form["reenterPassword"];
            if (password.CompareTo(reEnterPassword) == 0)
            {

                int nguoiDungID = LoggedInUser.NguoidungID;
                var nguoiDung = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
                if (nguoiDung != null)
                {
                    String firstHash = String.Concat(UserInfo.CreateMD5(password).ToLower(), password);
                    String md5MatKhau = UserInfo.CreateMD5(firstHash);
                    nguoiDung.Matkhau = md5MatKhau.ToLower();
                    db.Entry(nguoiDung).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Cập nhật profile thành công";
                    return RedirectToAction("editprofile", "home");
                }
            }

            int _nguoiDungID = Convert.ToInt32(Session["nguoiDungID"]);
            var _nguoiDung = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == _nguoiDungID);
            //kiểm tra xem có assoc được không
            int? nhanVienID = _nguoiDung.NhanvienID;
            if (nhanVienID != null)
            {
                //lấy thông tin nhân viên
                var nhanVien = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                ViewData["nhanVien"] = nhanVien;
            }
            else
            {
                ViewData["nhanVien"] = new Nhanvien();
            }
            ViewData["nguoiDung"] = _nguoiDung;

            ViewBag.identicalPassword = false;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Hàm đăng xuất ra khỏi hệ thống
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            //hủy toàn bộ session và redirect về trang login
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Lấy user session khi đăng nhập hệ thống
        /// </summary>

    }
}