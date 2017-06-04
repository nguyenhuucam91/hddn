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
using HoaDonNuocHaDong.Helper;
using System.Diagnostics;
using System.Data.Entity.Validation;
using HvitFramework;
using System.Data.SqlClient;
using HoaDonNuocHaDong.Models.KhachHang;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HDNHD.Models.Constants;
using System.Configuration;
using HoaDonNuocHaDong.Models;
using HoaDonHaDong.Helper;
using HDNHD.Models.DataContexts;


namespace HoaDonNuocHaDong.Controllers
{
    public class KhachhangController : BaseController
    {
        private Tuyen tuyenHelper = new Tuyen();
        private KhachHang khachHangHelper = new KhachHang();
        private SoLieuTieuThuController sLTT = new SoLieuTieuThuController();
        private NguoidungHelper ngDungHelper = new NguoidungHelper();
        private KiemDinh kiemDinhHelper = new KiemDinh();
        private ChiSo chiSo = new ChiSo();
        private KhachHangModel khachHangModel = new KhachHangModel();
        private LichSuHoaDonRepository lichSuHoaDonRepo = new LichSuHoaDonRepository();

        public static string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;

        const int ADMIN = 0;
        const int TRUONG_PHONG = 2;
        const int NHAN_VIEN = 1;

        public ActionResult Index(string TinhTrang = null, string catNuoc = null)
        {

            ViewBag.showKhachHang = false;
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

            List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.to = toLs;
            //load danh sách nhân viên thuộc tổ có phòng ban đó.
            List<Nhanvien> nVLs = new List<Nhanvien>();
            foreach (var item in toLs)
            {
                List<Nhanvien> _nvLs = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == item.ToQuanHuyenID && (p.IsDelete == false || p.IsDelete == null) && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
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

            if (TempData["to"] != null && TempData["nhanvien"] != null && TempData["tuyen"] != null)
            {
                int nhanVienIDFromIndex = Convert.ToInt32(TempData["nhanvien"]);
                int tuyenIDFromIndex = Convert.ToInt32(TempData["tuyen"]);
                int selectedToInt = Convert.ToInt32(TempData["to"]);
                IEnumerable<Khachhang> khachHangIQueryable = retrieveKhachHangFromFilter(tuyenIDFromIndex.ToString(), TinhTrang, catNuoc);
                ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
                ViewBag.selectedNhanVien = nhanVienIDFromIndex;
                ViewBag.selectedTuyen = tuyenIDFromIndex;
                ViewBag.selectedTo = selectedToInt;
                ViewBag.showKhachHang = true;
            }

            if (TempData["selectedNhanvien"] != null)
            {
                int selectedNhanVienTmpData = Convert.ToInt32(TempData["selectedNhanvien"]);
                int selectedTuyenTmpData = Convert.ToInt32(TempData["selectedTuyen"]);
                if (selectedNhanVienTmpData != 0)
                {
                    var tuyenTheoNhanVien = from i in db.Tuyentheonhanviens
                                            join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                            join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                            where i.NhanVienID == selectedNhanVienTmpData
                                            select r;

                    IEnumerable<Khachhang> khachHangIQueryable = retrieveKhachHangFromFilter(Session["selectedTuyen"].ToString(), TinhTrang, catNuoc);
                    ViewBag.tuyen = tuyenTheoNhanVien.ToList();
                    ViewBag.selectedNhanVien = selectedNhanVienTmpData;
                    ViewBag.selectedTuyen = selectedTuyenTmpData;
                    ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
                    ViewBag.showKhachHang = true;
                }
            }

            ViewBag.selectedChiNhanh = quanHuyenID;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            ViewBag.tuyen = tuyensLs;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form, string TinhTrang = null, string catNuoc = null)
        {
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));

            int phongBanID = getPhongBanNguoiDung();

            int toForm = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            int tuyenID = 0;

            List<Tuyenkhachhang> tuyenKH = new List<Tuyenkhachhang>();
            if (Session["tuyenID"] != null)
            {
                String[] tuyenIDSession = Session["tuyenID"].ToString().Split(',');
                foreach (var item in tuyenIDSession)
                {
                    tuyenID = Convert.ToInt32(item);
                    if (tuyenID != 0)
                    {
                        tuyenKH.Add(db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == tuyenID));
                    }
                }
            }


            int nhanVienID = String.IsNullOrEmpty(form["nhanvien"]) ? 0 : Convert.ToInt32(form["nhanvien"]);
            int tuyen = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            //danh sách viewBag
            int phongBanIDObjectInt = phongBanID;
            /*View Bag cho dropdown*/
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            //load viewbag mặc định
            ViewBag.chiNhanh = db.Chinhanhs.OrderBy(p => p.Ten).ToList();
            ViewBag.to = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.nhanVien = getNhanViensByTo(toForm);
            var tuyenTheoNhanVien = from i in db.Tuyentheonhanviens
                                    join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                    join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                    join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                    where i.NhanVienID == nhanVienID
                                    select r;
            ViewBag.tuyen = tuyenTheoNhanVien.ToList();
            ViewBag.tuyenTheoNhanVien = tuyenKH;
            ViewBag.showKhachHang = true;
            //selectedNhanvien và tuyến
            ViewBag.selectedNhanVien = nhanVienID;
            ViewBag.selectedTuyen = tuyen;
            ViewBag.selectedTo = toForm;

            // tiến hành lọc khách hàng dựa trên các tiêu chí trên
            if (tuyen > 0)
            {
                IEnumerable<Khachhang> khachHangIQueryable = retrieveKhachHangFromFilter("" + tuyen, TinhTrang, catNuoc);
                ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
            }
            else
            {
                if (nhanVienID > 0)
                {
                    IEnumerable<Khachhang> khachHangIQueryable = nonTuyenCustomers(nhanVienID, TinhTrang, catNuoc);
                    ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
                }
                else
                {
                    IEnumerable<Khachhang> khachHangIQueryable = nonNhanvienCustomers(toForm, TinhTrang, catNuoc);
                    ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
                }
            }
            return View();
        }

        private IEnumerable<Khachhang> nonNhanvienCustomers(int toForm, string TinhTrang, string catNuoc)
        {
            var nhanviens = db.Nhanviens.Where(p => p.IsDelete == false && p.ToQuanHuyenID == toForm);
            List<Khachhang> khachHangs = new List<Khachhang>();
            foreach (var nhanvien in nhanviens)
            {
                khachHangs.AddRange(nonTuyenCustomers(nhanvien.NhanvienID, TinhTrang, catNuoc));
            }
            return khachHangs;
        }

        private IEnumerable<Khachhang> nonTuyenCustomers(int nhanVienID, string TinhTrang, string catNuoc)
        {
            var tuyenTheoNhanVien = from i in db.Tuyentheonhanviens
                                    join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                    join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                    join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                    where i.NhanVienID == nhanVienID
                                    select r;
            List<Khachhang> khachHangs = new List<Khachhang>();
            foreach (var item in tuyenTheoNhanVien)
            {
                khachHangs.AddRange(retrieveKhachHangFromFilter(item.TuyenKHID.ToString(), TinhTrang, catNuoc).ToList());
            }
            return khachHangs;
        }

        /// <summary>
        /// lấy danh sách khách hàng từ filter ra
        /// </summary>
        /// <param name="tuyen"></param>
        /// <param name="TinhTrang"></param>
        /// <returns></returns>
        public IEnumerable<Khachhang> retrieveKhachHangFromFilter(string tuyen, string TinhTrang = null, string catNuoc = null)
        {

            var khachhangs = (from i in db.Khachhangs
                              join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                              where i.IsDelete == false && i.TuyenKHID.ToString() == tuyen
                              select new
                              {
                                  KhachHang = i,
                                  TuyenID = i.TuyenKHID,
                                  Tinhtrang = i.Tinhtrang,
                                  MaKhachHang = i.MaKhachHang,
                                  NgayCatNuoc = i.Ngayngungcapnuoc,
                                  NgayCapNuocLai = i.Ngaycapnuoclai,
                              });

            //set giá trị mặc định khi ko chọn một tiêu chí lọc cụ thể
            IQueryable<Khachhang> khachHangIQueryable = khachhangs.Select(p => p.KhachHang);

            if (String.IsNullOrEmpty(TinhTrang) || TinhTrang == "0" || String.IsNullOrEmpty(Request.QueryString["TinhTrang"]))
            {
                khachHangIQueryable = khachHangIQueryable.Where(p => p.Tinhtrang == 0);
                //KH bị cắt nước khi và chỉ khi ngày cắt nước <= ngày hiện tại
                if (!String.IsNullOrEmpty(catNuoc) && catNuoc == "1")
                {
                    khachHangIQueryable = khachHangIQueryable.Where(p => p.Ngayngungcapnuoc <= DateTime.Now && p.Ngaycapnuoclai == null);
                }
                //đang sử dụng nước <==> ngày cắt nước = null hoặc ngày cấp nước trở lại <= ngày hiện tại
                else
                {
                    khachHangIQueryable = khachHangIQueryable.Where(p => p.Ngayngungcapnuoc == null || p.Ngaycapnuoclai <= DateTime.Now);
                }
            }
            else
            {
                khachHangIQueryable = khachHangIQueryable.Where(p => p.Tinhtrang.ToString() == "1");
            }

            return khachHangIQueryable;

        }

        /// <summary>
        /// Khi Quận thay đổi thì phường thay đổi
        /// </summary>
        /// <param name="ChiNhanhID"></param>
        /// <returns>Chuỗi JSON</returns>
        public JsonResult FillPhuong(int QuanhuyenID)
        {
            var to = (from i in db.Phuongxas
                      where i.QuanhuyenID == QuanhuyenID && i.IsDelete == false
                      select new
                      {
                          Ten = i.Ten,
                          PhuongXaID = i.PhuongxaID
                      }).Distinct().ToList();
            return Json(to, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Khi Chi nhánh thay đổi thì phòng ban thay đổi theo chi nhánh
        /// </summary>
        /// <param name="ChiNhanhID"></param>
        /// <returns>Chuỗi JSON</returns>
        public JsonResult FillTo(int ChiNhanhID)
        {
            var to = (from i in db.Phongbans
                      where i.ChinhanhID == ChiNhanhID
                      select new
                      {
                          Ten = i.Ten,
                          ToID = i.PhongbanID
                      }).Distinct().ToList();
            return Json(to, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Khi Chi nhánh thay đổi thì phòng ban thay đổi theo chi nhánh
        /// </summary>
        /// <param name="ChiNhanhID"></param>
        /// <returns>Chuỗi JSON</returns>
        public JsonResult FillToByQuan(int ChiNhanhID, int? PhongBanAjax)
        {
            /** congnv - 170403 - trả về list tổ kinh doanh nếu vai trò ng dùng là inhoadon */
            if (HDNHD.Core.Models.RequestScope.UserRole == EUserRole.InHoaDon)
            {
                var phongBanRepository = uow.Repository<PhongBanRepository>();
                var toRepository = uow.Repository<ToRepository>();

                var phongKD = phongBanRepository.GetSingle(m => m.Ten.ToLower().Contains("kinh"));
                if (phongKD != null)
                {
                    var tos = toRepository.GetAll(m => m.QuanHuyenID == ChiNhanhID && m.IsDelete == false && m.PhongbanID == phongKD.PhongbanID)
                        .Select(m => new
                        {
                            Ten = m.Ma,
                            ToID = m.ToQuanHuyenID
                        }).Distinct().ToList();

                    return Json(tos, JsonRequestBehavior.AllowGet);
                }
            }
            /** END congnv - 170403 - trả về list tổ kinh doanh nếu vai trò ng dùng là inhoadon */

            int phongBanId = getPhongBanNguoiDung();
            if (phongBanId == 0)
            {
                var to = (from i in db.ToQuanHuyens
                          where i.QuanHuyenID == ChiNhanhID && i.IsDelete == false && i.PhongbanID == PhongBanAjax
                          select new
                          {
                              Ten = i.Ma,
                              ToID = i.ToQuanHuyenID
                          }).Distinct().ToList();
                return Json(to, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var to = (from i in db.ToQuanHuyens
                          where i.QuanHuyenID == ChiNhanhID && i.IsDelete == false && i.PhongbanID == phongBanId
                          select new
                          {
                              Ten = i.Ma,
                              ToID = i.ToQuanHuyenID
                          }).Distinct().ToList();
                return Json(to, JsonRequestBehavior.AllowGet);
            }

        }



        /// <summary>
        /// Fill tuyến theo quận
        /// </summary>
        /// <param name="QuanID"></param>
        /// <returns></returns>
        public JsonResult FillTuyenByQuan(int QuanID)
        {
            var tuyen = (from q in db.Quanhuyens
                         join r in db.ToQuanHuyens on q.QuanhuyenID equals r.QuanHuyenID
                         join s in db.Nhanviens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                         join t in db.Tuyentheonhanviens on s.NhanvienID equals t.NhanVienID
                         join p in db.Tuyenkhachhangs on t.TuyenKHID equals p.TuyenKHID
                         where q.QuanhuyenID == QuanID && (q.IsDelete == false || p.IsDelete == null)
                         select new
                         {
                             TuyenKHID = p.TuyenKHID,
                             Ten = p.Ten,
                         }).Distinct();
            return Json(tuyen, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkTTDoc(int? TTDoc, int? tuyenID)
        {
            var ttDoc = db.Khachhangs.Count(p => p.TTDoc == TTDoc && p.TuyenKHID == tuyenID && (p.IsDelete == false || p.IsDelete == null));
            return Json(ttDoc, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Load danh sách nhân viên theo Tổ (?) cần sửa lại
        /// </summary>
        /// <param name="ToID"></param>
        /// <returns></returns>
        public JsonResult FillNhanVien(int ToID)
        {
            var _nhanVien = (from i in db.Nhanviens
                             where i.PhongbanID == ToID && i.IsDelete == false
                             select new
                             {
                                 Ten = i.Ten,
                                 NhanvienID = i.NhanvienID
                             }).Distinct().ToList();
            return Json(_nhanVien, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillCumDanCu(int PhuongID)
        {
            var _nhanVien = (from i in db.Cumdancus
                             where i.PhuongxaID == PhuongID && i.IsDelete == false
                             select new
                             {
                                 Ten = i.Ten,
                                 CumdancuID = i.CumdancuID,
                             }).Distinct().ToList();
            return Json(_nhanVien, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load danh sách khách hàng theo tổ, loại bỏ KH bị xóa, bị thanh lý hợp đồng 
        /// </summary>
        /// <param name="ToID"></param>
        /// <returns></returns>
        public JsonResult FillNguoiDung(int TuyenID)
        {
            var _nhanVien = (from i in db.Khachhangs
                             where i.TuyenKHID == TuyenID && i.IsDelete == false && i.Tinhtrang == 0
                             select new
                             {
                                 Ten = i.Ten,
                                 MaKhachHang = i.MaKhachHang,
                                 KhachhangID = i.KhachhangID
                             }).Distinct().ToList();
            return Json(_nhanVien, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách tuyến theo nhân viên
        /// </summary>
        /// <param name="NhanVienID"></param>
        /// <returns></returns>
        public JsonResult FillTuyen(int? NhanVienID)
        {
            int userRole = getUserRole(LoggedInUser.NhanvienID);
            int quanHuyenCurrentLoggedInUser = getQuanHuyenOfLoggedInUser();
            int phongBanCurrentLoggedInUser = getPhongBanNguoiDung();
            List<ToQuanHuyen> tos = getToes(quanHuyenCurrentLoggedInUser, phongBanCurrentLoggedInUser);
            List<ModelNhanVien> nhanviens = new List<ModelNhanVien>();
            List<ModelTuyen> tuyens = new List<ModelTuyen>();
            foreach (var to in tos)
            {
                List<ModelNhanVien> nhanVien = getNhanViensByTo(to.ToQuanHuyenID);
                nhanviens.AddRange(nhanVien);
            }

            if (userRole == NHAN_VIEN)
            {
                tuyens = getTuyensThuocNhanVien(NhanVienID);
            }
            else
            {
                int selectedUserRole = getUserRole(NhanVienID);
                if (selectedUserRole == NHAN_VIEN)
                {
                    tuyens = getTuyensThuocNhanVien(NhanVienID);
                }
                else
                {
                    foreach (var nhanvien in nhanviens)
                    {
                        List<ModelTuyen> tuyensThuocNhanVien = getTuyensThuocNhanVien(nhanvien.NhanvienID);
                        tuyens.AddRange(tuyensThuocNhanVien);
                    }
                }
            }
            return Json(tuyens.Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách tuyến theo nhân viên
        /// </summary>
        /// <param name="NhanVienID"></param>
        /// <returns></returns>
        public JsonResult FillNhanVienByTo(int? ToID)
        {
            int isAdminAndTruongPhong = getUserRole(LoggedInUser.NhanvienID);
            var nhanViens = getNhanViensByTo(ToID);
            if (isAdminAndTruongPhong != ADMIN && isAdminAndTruongPhong != TRUONG_PHONG) //2: Truong phong
            {
                nhanViens = nhanViens.Where(p => p.ChucvuID == 1).ToList();
                return Json(nhanViens, JsonRequestBehavior.AllowGet);
            }
            return Json(nhanViens, JsonRequestBehavior.AllowGet);
        }



        // GET: /Khachhang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khachhang khachhang = db.Khachhangs.Find(id);

            if (khachhang == null)
            {
                return HttpNotFound();
            }
            return View(khachhang);
        }

        // GET: /Khachhang/Create
        public ActionResult Create()
        {
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;
            //lấy danh sách tuyến thuộc cả 1 tổ
            String nhanVienSession = LoggedInUser.NhanvienID.ToString();
            //lấy danh sách tuyến thuộc nhân viên ID
            if (!String.IsNullOrEmpty(nhanVienSession))
            {
                //lấy danh sách tổ của người dùng
                List<Models.TuyenKhachHang.TuyenKhachHang> _tuyen = new List<Models.TuyenKhachHang.TuyenKhachHang>();
                var toQuanHuyens = db.ToQuanHuyens.Where(p => p.PhongbanID == phongBanID && p.QuanHuyenID == quanHuyenID);
                int count = toQuanHuyens.Count();
                foreach (var item in toQuanHuyens)
                {
                    _tuyen.AddRange(tuyenHelper.getTuyenByTo(item.ToQuanHuyenID));
                }
                ViewBag.TuyenKHID = _tuyen;
            }

            ViewBag.selectedQuanHuyen = selectedQuanHuyenID;
            ViewBag.selectedQuanHuyenName = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            List<Phuongxa> phuongXas = db.Phuongxas.Where(p => p.QuanhuyenID == selectedQuanHuyenID && p.IsDelete == false).ToList();
            if (phuongXas.Count > 0)
            {
                int idPhuongXaDauTien = phuongXas.First().PhuongxaID;
                ViewBag.CumdancuID = db.Cumdancus.Where(p => p.IsDelete == false && p.PhuongxaID == idPhuongXaDauTien).ToList();
            }
            else
            {
                ViewBag.CumdancuID = new List<Phuongxa>();
            }
            ViewBag.HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten");
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int)EApGia.DacBiet), "LoaiapgiaID", "Ten");
            ViewBag.LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten");
            ViewBag.PhuongxaID = phuongXas;
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList(), "QuanhuyenID", "Ten");
            ViewBag.TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);
            ViewBag.MaKH = getMaxMaKhachHang();
            return View();
        }

        public int getMaxMaKhachHang()
        {
            SqlDataReader reader;
            int maKhachHangLonNhat = 0;
            int maKhachHangLonNhatTiepTheo = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "SELECT TOP 1 MaKhachHang FROM dbo.Khachhang ORDER BY KhachhangID DESC";
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    maKhachHangLonNhat = Convert.ToInt32(reader["MaKhachHang"]);
                }
                int incrementStep = 1;
                while (incrementStep >= 1)
                {
                    maKhachHangLonNhatTiepTheo = maKhachHangLonNhat + incrementStep;
                    if (!isMaKhachHangExistedInDB(maKhachHangLonNhatTiepTheo))
                    {
                        return maKhachHangLonNhatTiepTheo;
                    }
                    incrementStep++;
                }
                connection.Close();
            }

            return 0;
        }

        public bool isMaKhachHangExistedInDB(int maKhachHang)
        {
            int numberOfRowExisted = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "SELECT MaKhachHang FROM dbo.Khachhang WHERE MaKhachHang = @maKH";
                command.Parameters.AddWithValue("@maKH", maKhachHang);
                numberOfRowExisted = command.ExecuteNonQuery();
                connection.Close();
            }

            if (numberOfRowExisted > 0)
            {
                return true;
            }

            return false;
        }

        // POST: /Khachhang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KhachhangID,Makhachhang,QuanhuyenID,PhuongxaID,CumdancuID,TuyenKHID,LoaiKHID,LoaiapgiaID,HinhthucttID,TuyenongkythuatID,Sotaikhoan,Masothue,Ngaykyhopdong,Tilephimoitruong,Soho,Ngayap,Ngayhetap,Sonhankhau,Ten,Diachi,Dienthoai,Ghichu,Sokhuvuc,Sohopdong,Tinhtrang,Diachithutien,IsDelete,TTDoc")] Khachhang khachhang,
            FormCollection form)
        {
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            int ChiSoDau = String.IsNullOrEmpty(form["ChiSoDau"]) ? 0 : Convert.ToInt32(form["ChiSoDau"]);
            int maxMaKhachHang = getMaxMaKhachHang();
            khachhang.MaKhachHang = maxMaKhachHang.ToString();
            int thangKiHopDong = 0;
            int namKiHopDong = 0;
            if (ModelState.IsValid)
            {
                khachhang.Chisolapdat = ChiSoDau;
                //lấy thứ tự đọc
                int ttDoc = khachhang.TTDoc.Value;
                int tuyenID = khachhang.TuyenKHID.Value;
                int countCustomer = db.Khachhangs.Count(p => p.TTDoc == ttDoc && (p.IsDelete == false || p.IsDelete == null) && p.TuyenKHID == khachhang.TuyenKHID);
                thangKiHopDong = khachhang.Ngaykyhopdong.Value.Month;
                namKiHopDong = khachhang.Ngaykyhopdong.Value.Year;
                if (countCustomer > 0)
                {
                    khachHangHelper.pushKhachHangXuong(ttDoc, tuyenID);
                }
                //đặt tình trạng đang sử dụng
                khachhang.Tinhtrang = 0;
                maxMaKhachHang = getMaxMaKhachHang();
                khachhang.MaKhachHang = maxMaKhachHang.ToString();
                db.Khachhangs.Add(khachhang);
                // lưu thay đổi vào DB và bắt ngoại lệ để debug                            
                db.SaveChanges();
                //nếu là áp giá tổng hợp
                if (khachhang.LoaiapgiaID == KhachHang.TONGHOP)
                {
                    String loaiChiSo = form["loaiChiSo"];
                    double KD = form["KD"] == "" ? -1 : Convert.ToDouble(form["KD"]);
                    double SH = form["SH"] == "" ? -1 : Convert.ToDouble(form["SH"]);
                    double SX = form["SX"] == "" ? -1 : Convert.ToDouble(form["SX"]);
                    double HC = form["HC"] == "" ? -1 : Convert.ToDouble(form["HC"]);
                    double CC = form["CC"] == "" ? -1 : Convert.ToDouble(form["CC"]);
                    //trong trường hợp tính giá tổng hợp = 1 => tính theo %
                    if (form["loaiChiSo"] == "1")
                    {
                        KhachHang.saveGiaTongHop(khachhang.KhachhangID, 1, SH, KD, HC, CC, SX, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                    }
                    // tính theo chỉ số khoán
                    else
                    {
                        KhachHang.saveGiaTongHop(khachhang.KhachhangID, 0, SH, KD, HC, CC, SX, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                    }
                }

                //thêm mới trong bảng hóa đơn và chi tiết hóa đơn nước(tháng hiện tại)
                Hoadonnuoc hoaDonNuoc = new Hoadonnuoc();
                //lấy last inserted id của khách hàng
                hoaDonNuoc.KhachhangID = db.Khachhangs.Max(p => p.KhachhangID);
                //lấy nhân viên ID đang đăng nhập hệ thống
                if (LoggedInUser.NhanvienID != null)
                {
                    int nhanvienID = LoggedInUser.NhanvienID.Value;
                    hoaDonNuoc.NhanvienID = nhanvienID;
                }
                else
                {
                    hoaDonNuoc.NhanvienID = 0;
                }

                hoaDonNuoc.Tongsotieuthu = 0;
                hoaDonNuoc.Trangthaiin = false;
                hoaDonNuoc.Trangthaithu = false;
                hoaDonNuoc.Trangthaichot = false;
                hoaDonNuoc.Trangthaixoa = false;
                hoaDonNuoc.NamHoaDon = namKiHopDong;
                hoaDonNuoc.ThangHoaDon = thangKiHopDong;
                db.Hoadonnuocs.Add(hoaDonNuoc);
                db.SaveChanges();

                /*------------------CHI TIẾT HÓA ĐƠN NƯƠC tại tháng hiện tại------------*/
                themMoiChiTietHoaDonNuoc(ChiSoDau);
                ViewBag.successfulMessage = "Thêm mới khách hàng thành công";


                //nếu ấn nút quay lại thì mới chuyển về trang index, nếu không vẫn tiếp tục nhập
                String isBack = form["back"];
                if (isBack != null)
                {
                    TempData["tuyen"] = khachhang.TuyenKHID;
                    int nhanVienIDTuTuyen = tuyenHelper.getNhanVienIDTuTuyen(khachhang.TuyenKHID.Value);
                    TempData["nhanvien"] = nhanVienIDTuTuyen;
                    return RedirectToAction("Index");
                }
            }

            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;
            //lấy danh sách tuyến thuộc cả 1 tổ
            String nhanVienSession = LoggedInUser.NhanvienID.ToString();
            //lấy danh sách tuyến thuộc nhân viên ID
            if (!String.IsNullOrEmpty(nhanVienSession))
            {
                //lấy danh sách tổ của người dùng
                List<Models.TuyenKhachHang.TuyenKhachHang> _tuyen = new List<Models.TuyenKhachHang.TuyenKhachHang>();
                var toQuanHuyens = db.ToQuanHuyens.Where(p => p.PhongbanID == phongBanID && p.QuanHuyenID == quanHuyenID);
                int count = toQuanHuyens.Count();
                foreach (var item in toQuanHuyens)
                {
                    _tuyen.AddRange(tuyenHelper.getTuyenByTo(item.ToQuanHuyenID));
                }
                ViewBag.TuyenKHID = _tuyen;
            }

            ViewBag.selectedQuanHuyen = selectedQuanHuyenID;
            ViewBag.selectedQuanHuyenName = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            ViewBag.selectedCumDanCu = khachhang.CumdancuID;
            ViewBag.HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten", khachhang.HinhthucttID);
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int)EApGia.DacBiet), "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
            ViewBag.LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten", khachhang.LoaiKHID);

            List<Phuongxa> phuongXas = db.Phuongxas.Where(p => p.QuanhuyenID == selectedQuanHuyenID && p.IsDelete == false).ToList();
            if (phuongXas.Count > 0)
            {
                int idPhuongXaDauTien = phuongXas.First().PhuongxaID;
                ViewBag.CumdancuID = db.Cumdancus.Where(p => p.IsDelete == false && p.PhuongxaID == idPhuongXaDauTien).ToList();
            }
            else
            {
                ViewBag.CumdancuID = new List<Phuongxa>();
            }

            ViewBag.PhuongxaID = phuongXas;
            ViewBag.selectedPhuongXa = khachhang.PhuongxaID;
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false), "QuanhuyenID", "Ten", khachhang.QuanhuyenID);
            ViewBag.TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);
            ViewBag.selectedTuyenKHID = khachhang.TuyenKHID;
            ViewBag.reEnterCustomer = true;
            ViewBag.MaKH = maxMaKhachHang.ToString();
            return View(khachhang);
        }

        // GET: /Khachhang/Edit/5
        public ActionResult Edit(int? id)
        {
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khachhang khachhang = db.Khachhangs.Find(id);

            if (khachhang == null)
            {
                return HttpNotFound("Không tìm thấy khách hàng trong hệ thống");
            }

            //áp giá tổng hợp để đẩy vào ViewBag
            if (khachhang.LoaiapgiaID == KhachHang.TONGHOP || khachhang.LoaiapgiaID == KhachHang.DACBIET)
            {
                List<Apgiatonghop> ls = db.Apgiatonghops.Where(p => p.KhachhangID == id).ToList();
                if (ls != null)
                {
                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SINHHOAT) == null)
                    {
                        ViewBag.SH = "";
                    }
                    else
                    {
                        ViewBag.SH = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SINHHOAT).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.KINHDOANHDICHVU) == null)
                    {
                        ViewBag.KD = "";
                    }
                    else
                    {
                        ViewBag.KD = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.KINHDOANHDICHVU).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.COQUANHANHCHINH) == null)
                    {
                        ViewBag.HC = "";
                    }
                    else
                    {
                        ViewBag.HC = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.COQUANHANHCHINH).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.DONVISUNGHIEP) == null)
                    {
                        ViewBag.CC = "";
                    }
                    else
                    {
                        ViewBag.CC = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.DONVISUNGHIEP).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SANXUAT) == null)
                    {
                        ViewBag.SX = "";
                    }
                    else
                    {
                        ViewBag.SX = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SANXUAT).SanLuong.Value;
                    }
                    //nếu là áp giá tổng hợp
                    if (khachhang.LoaiapgiaID == KhachHang.TONGHOP)
                    {
                        if (ls.Count() > 0)
                        {
                            ViewBag.option = ls[0].CachTinh;
                        }
                        else
                        {
                            ViewBag.option = 0;
                        }

                        ViewBag.hasTongHop = true;
                        ViewBag.hasDacBiet = false;
                    }
                }

            }
            //nếu không phải loại giá tổng hợp & đặc biệt
            else
            {
                ViewBag.hasTongHop = false;
                ViewBag.hasDacBiet = false;
            }

            //lấy danh sách tuyến thuộc nhân viên ID
            if (nhanVien != null)
            {
                //lấy danh sách tổ của người dùng
                List<Models.TuyenKhachHang.TuyenKhachHang> _tuyen = new List<Models.TuyenKhachHang.TuyenKhachHang>();
                var toQuanHuyens = db.ToQuanHuyens.Where(p => p.PhongbanID == phongBanID && p.QuanHuyenID == quanHuyenID);
                int count = toQuanHuyens.Count();
                foreach (var item in toQuanHuyens)
                {
                    _tuyen.AddRange(tuyenHelper.getTuyenByTo(item.ToQuanHuyenID));
                }
                ViewBag._TuyenKHID = _tuyen;
                ViewBag.SelectedTuyen = khachhang.TuyenKHID;
            }

            ViewBag.selectedQuanHuyen = selectedQuanHuyenID;
            ViewBag.selectedQuanHuyenName = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            ViewBag.KHID = khachhang.KhachhangID;
            //thông tin dropdown
            ViewBag._CumdancuID = new SelectList(db.Cumdancus.Where(p => p.IsDelete == false), "CumdancuID", "Ten", khachhang.CumdancuID);
            ViewBag._HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten", khachhang.HinhthucttID);
            ViewBag._LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int)EApGia.DacBiet), "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
            ViewBag._LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten", khachhang.LoaiKHID);
            ViewBag._PhuongxaID = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongxaID", "Ten", khachhang.PhuongxaID);
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", khachhang.QuanhuyenID);
            ViewBag._TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);
            ViewBag.selectedTuyenOng = khachhang.TuyenongkythuatID;
            return View(khachhang);
        }

        // POST: /Khachhang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KhachhangID,Makhachhang,QuanhuyenID,PhuongxaID,CumdancuID,TuyenKHID,LoaiKHID,LoaiapgiaID,HinhthucttID,TuyenongkythuatID,Sotaikhoan,Masothue,Ngaykyhopdong,Tilephimoitruong,Soho,Ngayap,Ngayhetap,Sonhankhau,Ten,Diachi,Dienthoai,Ghichu,Sokhuvuc,Sohopdong,Tinhtrang,Diachithutien,IsDelete,TTDoc")] Khachhang khachhang,
            FormCollection form, int? toID, int? nhanVienIDUrl, int? tuyenIDUrl)
        {

            int selectedMonth = 0;
            int selectedYear = 0;
            if (Request.QueryString["thang"] != null)
            {
                selectedMonth = Convert.ToInt32(Request.QueryString["thang"]);
                selectedYear = Convert.ToInt32(Request.QueryString["nam"]);
            }
            else
            {
                selectedMonth = khachhang.Ngaykyhopdong.Value.Month;
                selectedYear = khachhang.Ngaykyhopdong.Value.Year;
            }

            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;
            int KHID = Convert.ToInt32(form["KhachhangID"]);
            khachhang.IsDelete = false;
            //lấy TTDoc trước
            int TTDocTruoc = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID).TTDoc.Value;

            if (ModelState.IsValid)
            {
                //cập nhật lại thứ tự đọc
                int ttDoc = khachhang.TTDoc.Value;
                int tuyenID = khachhang.TuyenKHID.Value;

                int countCustomer = db.Khachhangs.Count(p => p.TTDoc == ttDoc && (p.IsDelete == false || p.IsDelete == null) && p.TuyenKHID == khachhang.TuyenKHID);
                if (countCustomer > 0)
                {
                    KhachHang.pullKhachHangLen(ttDoc, TTDocTruoc, tuyenID);
                }
                //create new connection to save Db
                HoaDonHaDongEntities newConnection = new HoaDonHaDongEntities();
                newConnection.Entry(khachhang).State = EntityState.Modified;
                newConnection.SaveChanges();
                //cập nhật lại chỉ số đầu cho khách hàng trong trường hợp nhập sai
                int chiSoDauEdited = String.IsNullOrEmpty(form["ChiSoDau"]) ? -1 : Convert.ToInt32(form["ChiSoDau"]);
                //nếu chỉ số đầu để trống thì không cập nhật lại nữa
                Hoadonnuoc hoaDonNuoc = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachhang.KhachhangID && p.ThangHoaDon == selectedMonth && p.NamHoaDon == selectedYear);
                if (hoaDonNuoc != null)
                {
                    if (chiSoDauEdited != -1)
                    {
                        if (hoaDonNuoc != null)
                        {
                            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == hoaDonNuoc.HoadonnuocID);
                            chiTiet.Chisocu = chiSoDauEdited;
                            db.Entry(chiTiet).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    Hoadonnuoc hoaDonThangNamKiHopDong = new Hoadonnuoc();
                    //lấy last inserted id của khách hàng
                    hoaDonThangNamKiHopDong.KhachhangID = db.Khachhangs.Max(p => p.KhachhangID);
                    //lấy nhân viên ID đang đăng nhập hệ thống
                    if (LoggedInUser.NhanvienID != null)
                    {
                        int nhanvienID = LoggedInUser.NhanvienID.Value;
                        hoaDonThangNamKiHopDong.NhanvienID = nhanvienID;
                    }
                    else
                    {
                        hoaDonThangNamKiHopDong.NhanvienID = 0;
                    }

                    hoaDonThangNamKiHopDong.Tongsotieuthu = 0;
                    //trạng thái in & trạng thái thu = false (chưa in và chưa thu)
                    hoaDonThangNamKiHopDong.Trangthaiin = false;
                    hoaDonThangNamKiHopDong.Trangthaithu = false;
                    hoaDonThangNamKiHopDong.NamHoaDon = selectedYear;
                    hoaDonThangNamKiHopDong.ThangHoaDon = selectedMonth;
                    hoaDonThangNamKiHopDong.Ngaybatdausudung = khachhang.Ngaykyhopdong;
                    db.Hoadonnuocs.Add(hoaDonThangNamKiHopDong);
                    db.SaveChanges();
                    int chiSoDauChiTietHoaDon = 0;
                    themMoiChiTietHoaDonNuoc(chiSoDauChiTietHoaDon);
                }

                //xóa bảng áp giá tổng hơp trước
                List<Apgiatonghop> apTongHop = db.Apgiatonghops.Where(p => p.KhachhangID == khachhang.KhachhangID && p.ThangTongHop == selectedMonth && p.NamTongHop == selectedYear).ToList();
                if (apTongHop != null)
                {
                    db.Apgiatonghops.RemoveRange(apTongHop);
                    db.SaveChanges();
                }
                //tiến hành áp giá và chia lại chỉ số
                int loaiApGiaID = Convert.ToInt32(form["LoaiapgiaID"]);
                int sanLuongTieuThu = 0; int chiSoDau = 0; int chiSoCuoi = 0; int soKhoan = 0;
                //lấy sản lượng đã có trong db
                Hoadonnuoc hD = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachhang.KhachhangID && p.ThangHoaDon == selectedMonth && p.NamHoaDon == selectedYear);
                if (hD != null)
                {
                    int? isNullTongSoTieuThu = hD.Tongsotieuthu;
                    if (isNullTongSoTieuThu != null)
                    {
                        sanLuongTieuThu = isNullTongSoTieuThu.Value;
                    }
                    else
                    {
                        sanLuongTieuThu = 0;
                    }

                    //chi tiết
                    Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == hD.HoadonnuocID);
                    if (cT != null)
                    {
                        chiSoDau = cT.Chisocu.Value;
                        //nếu chưa nhập chỉ số cuối 
                        int? hasChiSoMoi = cT.Chisomoi;
                        if (hasChiSoMoi != null)
                        {
                            chiSoCuoi = hasChiSoMoi.Value;
                        }
                    }

                    //tách chỉ số tổng hợp
                    //tiến hành áp lại giá tổng hợp và đặc biệt
                    if (loaiApGiaID == KhachHang.TONGHOP)
                    {
                        String loaiChiSo = form["loaiChiSo"];
                        double KD = form["KD"] == "" ? -1 : Convert.ToDouble(form["KD"]);
                        double SH = form["SH"] == "" ? -1 : Convert.ToDouble(form["SH"]);
                        double SX = form["SX"] == "" ? -1 : Convert.ToDouble(form["SX"]);
                        double HC = form["HC"] == "" ? -1 : Convert.ToDouble(form["HC"]);
                        double CC = form["CC"] == "" ? -1 : Convert.ToDouble(form["CC"]);
                        //trong trường hợp tính giá tổng hợp = 1 => tính theo %
                        if (form["loaiChiSo"] == "1")
                        {
                            KhachHang.saveGiaTongHop(khachhang.KhachhangID, 1, SH, KD, HC, CC, SX, selectedMonth, Convert.ToInt16(selectedYear));
                            sLTT.tachSoTongHop(hD.HoadonnuocID, 1, khachhang.KhachhangID, sanLuongTieuThu);
                        }
                        // tính theo chỉ số khoán
                        else
                        {
                            KhachHang.saveGiaTongHop(khachhang.KhachhangID, 0, SH, KD, HC, CC, SX, selectedMonth, Convert.ToInt16(selectedYear));
                            sLTT.tachSoTongHop(hD.HoadonnuocID, 0, khachhang.KhachhangID, sanLuongTieuThu);
                        }
                        //chia lại giá                    
                    }
                    //tách lại chỉ số giá khác
                    else
                    {
                        sLTT.tachChiSoSanLuong(hD.HoadonnuocID, chiSoDau, chiSoCuoi, sanLuongTieuThu, soKhoan, khachhang.KhachhangID);
                    }

                    #region TongTienHoaDon
                    double dinhMuc = chiSo.tinhTongTienTheoDinhMuc(hD.HoadonnuocID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
                    double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
                    double thueBVMT = chiSo.tinhThue(hD.HoadonnuocID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, khachhang.Tilephimoitruong.Value);
                    double tongTienHoaDon = dinhMuc + thueBVMT + VAT;

                    if (tongTienHoaDon <= 0)
                    {
                        tongTienHoaDon = 0;
                    }

                    if (dinhMuc <= 0)
                    {
                        dinhMuc = 0;
                    }

                    if (VAT <= 0)
                    {
                        VAT = 0;
                    }

                    if (thueBVMT <= 0)
                    {
                        thueBVMT = 0;
                    }

                    String thuNgan = khachhang.TTDoc + "/" + tuyenHelper.getMaTuyenById(khachhang.TuyenKHID.Value) + " - " + 0;
                    String ngayBatDauSuDung = String.Format("{0: dd/MM/yyyy }", hoaDonNuoc.Ngaybatdausudung.Value);
                    String ngayKetThucSuDung = String.Format("{0: dd/MM/yyyy }", hoaDonNuoc.Ngayketthucsudung.Value);
                    #endregion

                    lichSuHoaDonRepo.updateLichSuHoaDon(hD.HoadonnuocID, selectedMonth, selectedYear, khachhang.Ten, khachhang.Diachi, khachhang.Masothue, khachhang.MaKhachHang,
                           khachhang.TuyenKHID.Value, khachhang.Sohopdong, chiSoDau, chiSoCuoi, sanLuongTieuThu,
                           cT.SH1.Value, chiSo.getSoTienTheoApGia("SH1").Value,
                           cT.SH2.Value, chiSo.getSoTienTheoApGia("SH2").Value,
                           cT.SH3.Value, chiSo.getSoTienTheoApGia("SH3").Value,
                           cT.SH4.Value, chiSo.getSoTienTheoApGia("SH4").Value,
                           cT.HC.Value, chiSo.getSoTienTheoApGia("HC").Value,
                           cT.CC.Value, chiSo.getSoTienTheoApGia("CC").Value,
                           cT.SXXD.Value, chiSo.getSoTienTheoApGia("SXXD").Value,
                           cT.KDDV.Value, chiSo.getSoTienTheoApGia("KDDV").Value,
                           5, VAT, khachhang.Tilephimoitruong.Value, thueBVMT, tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                           db.Quanhuyens.Find(khachhang.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(khachhang.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(khachhang.QuanhuyenID).DienThoai3,
                           thuNgan, khachhang.TuyenKHID.Value, khachhang.TTDoc.Value, 0, ngayBatDauSuDung,
                           ngayKetThucSuDung);
                } //end hD != null


                //chỉnh sửa khách hàng trong trang áp giá
                if (!String.IsNullOrEmpty(Request.QueryString["referrer"]) && Request.QueryString["referrer"] == "viewdetail")
                {
                    return RedirectToAction("viewdetail", "Solieutieuthu", new
                    {
                        toID = toID,
                        tuyenID = tuyenIDUrl,
                        month = Request.QueryString["thang"],
                        year = Request.QueryString["nam"],
                        nvquanly = nhanVienIDUrl
                    });
                }
                else if (!String.IsNullOrEmpty(Request.QueryString["referrer"]) && Request.QueryString["referrer"] == "solieutieuthuindex")
                {
                    return RedirectToAction("index", "solieutieuthu");
                }

                TempData["to"] = toID;
                TempData["tuyen"] = tuyenIDUrl;
                TempData["nhanvien"] = nhanVienIDUrl;
                //tiến hành đẩy các record xuống dựa theo TTDoc
                return RedirectToAction("Index");
            }

            //lấy danh sách tuyến thuộc nhân viên ID
            if (nhanVien != null)
            {
                //lấy danh sách tổ của người dùng
                List<Models.TuyenKhachHang.TuyenKhachHang> _tuyen = new List<Models.TuyenKhachHang.TuyenKhachHang>();
                var toQuanHuyens = db.ToQuanHuyens.Where(p => p.PhongbanID == phongBanID && p.QuanHuyenID == quanHuyenID);
                int count = toQuanHuyens.Count();
                foreach (var item in toQuanHuyens)
                {
                    _tuyen.AddRange(tuyenHelper.getTuyenByTo(item.ToQuanHuyenID));
                }
                ViewBag._TuyenKHID = _tuyen;
                ViewBag.SelectedTuyen = khachhang.TuyenKHID;
            }

            ViewBag._CumdancuID = new SelectList(db.Cumdancus.Where(p => p.IsDelete == false), "CumdancuID", "Ten", khachhang.CumdancuID);
            ViewBag._HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten", khachhang.HinhthucttID);
            ViewBag._LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int)EApGia.DacBiet), "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
            ViewBag._LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten", khachhang.LoaiKHID);
            ViewBag._PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten", khachhang.PhuongxaID);
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens, "QuanhuyenID", "Ten", khachhang.QuanhuyenID);
            ViewBag._TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);
            ViewBag.selectedTuyenOng = khachhang.TuyenongkythuatID;
            //loại khách hàng
            if (khachhang.LoaiapgiaID == KhachHang.TONGHOP || khachhang.LoaiapgiaID == KhachHang.DACBIET)
            {
                List<Apgiatonghop> ls = db.Apgiatonghops.Where(p => p.KhachhangID == khachhang.KhachhangID).ToList();
                if (ls != null)
                {
                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SINHHOAT) == null)
                    {
                        ViewBag.SH = "";
                    }
                    else
                    {
                        ViewBag.SH = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SINHHOAT).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.KINHDOANHDICHVU) == null)
                    {
                        ViewBag.KD = "";
                    }
                    else
                    {
                        ViewBag.KD = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.KINHDOANHDICHVU).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.COQUANHANHCHINH) == null)
                    {
                        ViewBag.HC = "";
                    }
                    else
                    {
                        ViewBag.HC = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.COQUANHANHCHINH).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.DONVISUNGHIEP) == null)
                    {
                        ViewBag.CC = "";
                    }
                    else
                    {
                        ViewBag.CC = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.DONVISUNGHIEP).SanLuong.Value;
                    }

                    if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SANXUAT) == null)
                    {
                        ViewBag.SX = "";
                    }
                    else
                    {
                        ViewBag.SX = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SANXUAT).SanLuong.Value;
                    }

                    //nếu là áp giá tổng hợp
                    if (khachhang.LoaiapgiaID == KhachHang.TONGHOP)
                    {
                        if (ls.Count() > 0)
                        {
                            ViewBag.option = ls[0].CachTinh;
                        }
                        else
                        {
                            ViewBag.option = 0;
                        }
                        ViewBag.hasTongHop = true;
                        ViewBag.hasDacBiet = false;
                    }

                    //nếu là áp giá đặc biệt
                    else if (khachhang.LoaiapgiaID == KhachHang.DACBIET)
                    {
                        if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH1) == null)
                        {
                            ViewBag.SH1 = "";
                        }
                        else
                        {
                            ViewBag.SH1 = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH1).SanLuong.Value;
                        }

                        if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH2) == null)
                        {
                            ViewBag.SH2 = "";
                        }
                        else
                        {
                            ViewBag.SH2 = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH2).SanLuong.Value;
                        }

                        if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH3) == null)
                        {
                            ViewBag.SH3 = "";
                        }
                        else
                        {
                            ViewBag.SH3 = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH3).SanLuong.Value;
                        }

                        if (ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH4) == null)
                        {
                            ViewBag.SH4 = "";
                        }
                        else
                        {
                            ViewBag.SH4 = ls.FirstOrDefault(p => p.IDLoaiApGia == KhachHang.SH4).SanLuong.Value;
                        }
                        ViewBag.hasTongHop = false;
                        ViewBag.hasDacBiet = true;
                    }
                }

            }
            //nếu không phải loại giá tổng hợp & đặc biệt
            else
            {
                ViewBag.hasTongHop = false;
                ViewBag.hasDacBiet = false;
            }
            return View(khachhang);
        }

        public void themMoiChiTietHoaDonNuoc(int ChiSoDau)
        {
            Chitiethoadonnuoc chiTiet = new Chitiethoadonnuoc();
            chiTiet.HoadonnuocID = db.Hoadonnuocs.Max(p => p.HoadonnuocID);
            chiTiet.Chisocu = ChiSoDau;
            chiTiet.Chisomoi = 0;
            chiTiet.SH1 = 0;
            chiTiet.SH2 = 0;
            chiTiet.SH3 = 0;
            chiTiet.SH4 = 0;
            chiTiet.HC = 0;
            chiTiet.CC = 0;
            chiTiet.SXXD = 0;
            chiTiet.KDDV = 0;
            db.Chitiethoadonnuocs.Add(chiTiet);
            db.SaveChanges();
        }

        // GET: /Khachhang/Delete/5
        /// <summary>
        /// Chuyển khách hàng về trạng thái chấm dứt sử dụng hơp đồng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id, int? toID, int? nhanvienID, int? tuyenID, string TinhTrang = null)
        {
            Khachhang khachhang = db.Khachhangs.Find(id);
            //chuyển về trạng thái ngưng sử dụng tạm thời
            khachhang.IsDelete = true;
            db.SaveChanges();

            List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == id && p.ThangHoaDon == DateTime.Now.Month
                && p.NamHoaDon == DateTime.Now.Year).ToList();
            foreach (var item in hoadons)
            {
                item.Trangthaixoa = true;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            TempData["nhanvien"] = nhanvienID;
            TempData["tuyen"] = tuyenID;
            TempData["to"] = toID;

            if (!String.IsNullOrEmpty(TinhTrang))
            {
                return RedirectToAction("Index", new { TinhTrang = TinhTrang });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Active(int id)
        {
            Khachhang khachhang = db.Khachhangs.Find(id);
            khachhang.Tinhtrang = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Hiện view thanh lý hợp đồng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult Inactive(int id, int? toID, int? nhanvienID, int? tuyenID)
        {
            Khachhang kH = db.Khachhangs.Find(id);
            ViewBag.KHID = id;
            ViewData["KhachHang"] = kH;
            return View(kH);
        }

        /// <summary>
        /// Hàm xử lí thanh lý hợp đồng, đồng thời đẩy trạng thái xóa của hóa đơn = true (không hiển thị lên hóa đơn)
        /// </summary>
        /// <param name="form"></param>
        /// <param name="id"></param>
        /// <param name="toID"></param>
        /// <param name="nhanvienID"></param>
        /// <param name="tuyenID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Inactive(FormCollection form, int? id, int? toID, int? nhanvienID, int? tuyenID)
        {
            String liDoThanhLy = form["Lydothanhly"];
            string[] hiddenKhachHang = form["thanhLy"].ToString().Split(',');
            foreach (var item in hiddenKhachHang)
            {
                int khachHangThanhLyID = Convert.ToInt32(item);
                //bằng 1 là đã thanh lý hơp đồng
                var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == khachHangThanhLyID);
                khachHang.Tinhtrang = 1;
                DateTime ngayThanhLy = Convert.ToDateTime(form["ngayThanhLy"]);
                khachHang.Ngaythanhly = ngayThanhLy;
                khachHang.Lydothanhly = liDoThanhLy;
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                //cập nhật lại trạng thái của hóa đơn, từ hiển thị => xóa
                List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangThanhLyID && p.ThangHoaDon == ngayThanhLy.Month && p.NamHoaDon == ngayThanhLy.Year).ToList();
                List<Hoadonnuoc> danhSachHoaDonSauNgayThanhLy = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangThanhLyID && p.Ngaybatdausudung >= ngayThanhLy).ToList();
                List<Hoadonnuoc> danhSachHoaDonNamGiuaNgayBatDauVaKetThuc = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangThanhLyID
                                                                            && p.Ngaybatdausudung <= ngayThanhLy
                                                                            && ngayThanhLy <= p.Ngayketthucsudung).ToList();
                hoadons.AddRange(danhSachHoaDonSauNgayThanhLy);
                foreach (var hoadon in hoadons)
                {
                    hoadon.Trangthaixoa = true;
                    db.Entry(hoadon).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["nhanvien"] = nhanvienID;
                TempData["tuyen"] = tuyenID;
                TempData["to"] = toID;
            }

            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (nhanvienID == null)
                {
                    return RedirectToAction("FilterMaKH");
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xem chi tiết thông tin của khách hàng nhất định
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewDetail(int? id)
        {
            Khachhang khachhang = db.Khachhangs.Where(p => p.KhachhangID == id).FirstOrDefault();
            if (khachhang != null)
            {
                ViewBag._CumdancuID = new SelectList(db.Cumdancus.Where(p => p.IsDelete == false), "CumdancuID", "Ten", khachhang.CumdancuID);
                ViewBag._HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten", khachhang.HinhthucttID);
                ViewBag._LoaiapgiaID = new SelectList(db.Loaiapgias, "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
                ViewBag._LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten", khachhang.LoaiKHID);
                ViewBag._PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten", khachhang.PhuongxaID);
                ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens, "QuanhuyenID", "Ten", khachhang.QuanhuyenID);
                ViewBag._TuyenKHID = new SelectList(db.Tuyenkhachhangs, "TuyenKHID", "Ten", khachhang.TuyenKHID);
                ViewBag._TuyenongkythuatID = new SelectList(db.Tuyenongs, "TuyenongID", "Tentuyen", khachhang.TuyenongkythuatID);

                //thông tin kiểm định
                ViewBag.kiemDinh = kiemDinhHelper.getDanhSachKiemDinhCuaKhachHang(id.Value);
                //thông tin chỉ số
                ViewBag.ttChiSo = (from i in db.Hoadonnuocs
                                   join r in db.Chitiethoadonnuocs on i.HoadonnuocID equals r.HoadonnuocID
                                   where i.KhachhangID == id.Value
                                   select new
                                   {
                                       HoaDonID = i.HoadonnuocID,
                                       NgayHoaDon = i.ThangHoaDon + "/" + i.NamHoaDon,
                                       ChiSoCu = r.Chisocu,
                                       ChiSoMoi = r.Chisomoi,
                                       SanLuong = i.Tongsotieuthu,
                                       GhiChu = i.Ghichu,
                                       SH1 = r.SH1,
                                       SH2 = r.SH2,
                                       SH3 = r.SH3,
                                       SH4 = r.SH4,
                                       HC = r.HC,
                                       CC = r.CC,
                                       SXXD = r.SXXD,
                                       KDDV = r.KDDV,
                                   });
                return View(khachhang);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CatNuoc(FormCollection form, int? toID, int? nhanvienID, int? tuyenID)
        {
            String lyDoNgungCapNuoc = form["Lydongungcapnuoc"];
            string[] hiddenKhachHang = form["ngungcapnuoc"].ToString().Split(',');
            foreach (var item in hiddenKhachHang)
            {
                int khachHangNgungCapNuocID = Convert.ToInt32(item);
                DateTime ngayNgungCapNuoc = Convert.ToDateTime(form["Ngayngungcapnuoc"]);
                //bằng 1 là đã thanh lý hơp đồng
                var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == khachHangNgungCapNuocID);
                khachHang.Ngayngungcapnuoc = ngayNgungCapNuoc;
                khachHang.Lydongungcapnuoc = lyDoNgungCapNuoc;
                khachHang.Lydocapnuoclai = null;
                khachHang.Ngaycapnuoclai = null;
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                //cập nhật lại trạng thái của hóa đơn, từ hiển thị => xóa
                //List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangNgungCapNuocID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year).ToList();
                //foreach (var hoadon in hoadons)
                //{
                //    hoadon.Trangthaixoa = true;
                //    db.Entry(hoadon).State = EntityState.Modified;
                //    db.SaveChanges();
                //}

                TempData["nhanvien"] = nhanvienID;
                TempData["tuyen"] = tuyenID;
                TempData["to"] = toID;
            }

            if (toID == null)
            {
                return RedirectToAction("FilterMaKH");
            }
            return RedirectToAction("Index");
        }

        public ActionResult CatNuoc(int id)
        {
            Khachhang kH = db.Khachhangs.Find(id);
            ViewBag.KHID = id;
            ViewData["KhachHang"] = kH;
            return View(kH);
        }

        /// <summary>
        /// Câp lại nước cho khách hàng cắt nước
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tinhTrang"></param>
        /// <returns></returns>
        public ActionResult Caplainuoc(int id, string tinhTrang)
        {
            Khachhang kH = db.Khachhangs.Find(id);
            ViewBag.KHID = id;
            ViewData["KhachHang"] = kH;
            return View(kH);
        }

        [HttpPost]
        public ActionResult Caplainuoc(FormCollection form, int? toID, int? nhanvienID, int? tuyenID)
        {
            String lyDoCapNuocLai = form["Lydocapnuoclai"];
            string[] hiddenKhachHang = form["capnuoclai"].ToString().Split(',');
            DateTime ngayCapNuocLai = Convert.ToDateTime(form["Ngaycapnuoclai"]);
            foreach (var item in hiddenKhachHang)
            {
                int khachHangNgungCapNuocID = Convert.ToInt32(item);
                //bằng 1 là đã thanh lý hơp đồng
                var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == khachHangNgungCapNuocID);
                khachHang.Ngaycapnuoclai = ngayCapNuocLai;
                khachHang.Lydocapnuoclai = lyDoCapNuocLai;
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();

                List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangNgungCapNuocID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year).ToList();
                foreach (var hoadon in hoadons)
                {
                    hoadon.Trangthaixoa = false;
                    db.Entry(hoadon).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["to"] = toID;
                TempData["nhanvien"] = nhanvienID;
                TempData["tuyen"] = tuyenID;
            }

            if (toID == null)
            {
                return RedirectToAction("FilterMaKH");
            }

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

        /// <summary>
        /// Lọc khách hàng theo mã khách hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult FilterMaKH()
        {
            ViewData["khachhangAfterFiltered"] = new List<Khachhang>();
            return View();
        }

        [HttpPost]
        public ActionResult FilterMaKH(FormCollection form)
        {
            String filterString = String.IsNullOrEmpty(form["filterString"]) ? "" : form["filterString"];
            int filterCriteria = !String.IsNullOrEmpty(form["filterCriteria"]) ? Convert.ToInt16(form["filterCriteria"]) : 1;
            var khachHangs = new List<Khachhang>();
            switch (filterCriteria)
            {
                case (int)ECustomerFilterCriteria.MA_KHACH_HANG:
                    khachHangs = khachHangModel.filterByMaKhachHang(filterString);
                    break;
                case (int)ECustomerFilterCriteria.TEN_KHACH_HANG:
                    khachHangs = khachHangModel.filterByTenKhachHang(filterString);
                    break;
                case (int)ECustomerFilterCriteria.SO_HOP_DONG:
                    khachHangs = khachHangModel.filterBySoHopDong(filterString);
                    break;
                case (int)ECustomerFilterCriteria.DIA_CHI:
                    khachHangs = khachHangModel.filterByDiaChi(filterString);
                    break;
            }

            #region ViewData
            ViewData["khachhangAfterFiltered"] = khachHangs;
            #endregion
            return View();
        }
    }
}