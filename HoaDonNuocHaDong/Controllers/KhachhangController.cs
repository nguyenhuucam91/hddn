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


namespace HoaDonNuocHaDong.Controllers
{
    public class KhachhangController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        Tuyen tuyenHelper = new Tuyen();
        KhachHang khachHangHelper = new KhachHang();
        SoLieuTieuThuController sLTT = new SoLieuTieuThuController();
        NguoidungHelper ngDungHelper = new NguoidungHelper();
        /// <summary>
        /// Hàm kiểm tra session trước khi đăng nhập
        /// </summary>
        /// <param name="filterContext"></param>       
        public static int makh { get; private set; }
        public ActionResult Index(string TinhTrang = null, string catNuoc = null)
        {
            //xem danh sách khách hàng
            ViewBag.showKhachHang = false;
            //code lại từ đầu
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            ViewBag.selectedChiNhanh = quanHuyenID;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);


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
            ViewBag.tuyen = tuyensLs;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form, string TinhTrang = null, string catNuoc = null)
        {
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));

            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

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
            ViewBag.nhanVien = db.Nhanviens.Where(p => p.ToQuanHuyenID == toForm && (p.IsDelete == false || p.IsDelete == null)).ToList();
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


            int _tinhTrangSuDung = String.IsNullOrEmpty(TinhTrang) ? 0 : Convert.ToInt32(TinhTrang);
            int _tinhTrangCatNuoc = String.IsNullOrEmpty(catNuoc) ? 0 : Convert.ToInt32(catNuoc);
            //nếu tình trạng sử dụng == 1 (thanh lý) thì đặt tình trạng cắt nước =  cắt nước
            if (_tinhTrangSuDung == 1)
            {
                _tinhTrangCatNuoc = 1;
            }
            // tiến hành lọc khách hàng dựa trên các tiêu chí trên
            if (nhanVienID == 0 && tuyen == 0)
            {
                ControllerBase<HoaDonNuocHaDong.Models.KhachHang.KhachHangModel> cb = new ControllerBase<Models.KhachHang.KhachHangModel>();
                List<HoaDonNuocHaDong.Models.KhachHang.KhachHangModel> ls = cb.Query("DSKH",
                    new SqlParameter("@d1", _tinhTrangSuDung)
                    ).Distinct().ToList();

                DateTime defaultDate = Convert.ToDateTime("0001/01/01");
                //cắt nước rồi
                if (TinhTrang != "1")
                {
                    if (catNuoc == "1")
                    {
                        ls = ls.Where(p => p.Ngayngungcapnuoc <= DateTime.Now && p.Ngayngungcapnuoc > defaultDate
                            && p.Ngaycapnuoclai == defaultDate).ToList();
                    }
                    //chưa cắt nước
                    else
                    {
                        ls = ls.Where(p => p.Ngayngungcapnuoc == defaultDate || p.Ngayngungcapnuoc == null || p.Ngaycapnuoclai <= DateTime.Now).ToList();
                    }
                }
                ViewData["khachHang"] = ls;

                return View("TongDanhSach");
            }
            //nếu nhân viên và tuyến được chọn
            else
            {
                IEnumerable<Khachhang> khachHangIQueryable = retrieveKhachHangFromFilter("" + tuyen, TinhTrang, catNuoc);
                ViewBag.khachHang = khachHangIQueryable.OrderBy(p => p.TTDoc).ToList();
            }
            return View();
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
                              //join bảng tuyến khách hàng (*)
                              join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                              join t in db.ToQuanHuyens on i.QuanhuyenID equals t.ToQuanHuyenID
                              where i.IsDelete == false || i.IsDelete == null
                              select new
                              {
                                  KhachHang = i,
                                  TuyenID = i.TuyenKHID,
                                  Tinhtrang = i.Tinhtrang,
                                  MaKhachHang = i.MaKhachHang,
                                  NgayCatNuoc = i.Ngayngungcapnuoc,
                                  NgayCapNuocLai = i.Ngaycapnuoclai,
                              });

            
            if (String.IsNullOrEmpty(TinhTrang) || TinhTrang == "0" || String.IsNullOrEmpty(Request.QueryString["TinhTrang"]))
            {
                khachhangs = khachhangs.Where(p => p.Tinhtrang == 0 || p.Tinhtrang == null);
            }
            else
            {
                int tinhTrang = int.Parse(TinhTrang);
                khachhangs = khachhangs.Where(p => p.Tinhtrang == tinhTrang);
            }

            if (TinhTrang != "1")
            {
                //KH bị cắt nước khi và chỉ khi ngày cắt nước <= ngày hiện tại
                if (!String.IsNullOrEmpty(catNuoc) && catNuoc == "1")
                {
                    khachhangs = khachhangs.Where(p => p.NgayCatNuoc <= DateTime.Now && p.NgayCapNuocLai == null);
                }
                //đang sử dụng nước <==> ngày cắt nước = null hoặc ngày cấp nước trở lại <= ngày hiện tại
                else
                {
                    khachhangs = khachhangs.Where(p => p.NgayCatNuoc == null || p.NgayCapNuocLai <= DateTime.Now);
                }
            }
            //set giá trị mặc định khi ko chọn một tiêu chí lọc cụ thể
            IQueryable<Khachhang> khachHangIQueryable = khachhangs.Select(p => p.KhachHang);
            //nếu tuyến không rỗng (được chọn)
            if (tuyen != null)
            {
                int selectedTuyenID = Convert.ToInt32(tuyen);
                khachHangIQueryable = khachhangs.Select(p => p.KhachHang).Where(p => p.TuyenKHID == selectedTuyenID).Distinct();
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
        public JsonResult FillToByQuan(int ChiNhanhID)
        {
            var to = (from i in db.ToQuanHuyens
                      where i.QuanHuyenID == ChiNhanhID && i.IsDelete == false
                      select new
                      {
                          Ten = i.Ma,
                          ToID = i.ToQuanHuyenID
                      }).Distinct().ToList();
            return Json(to, JsonRequestBehavior.AllowGet);
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
            var tuyen = (from i in db.Tuyentheonhanviens
                         join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                         where i.NhanVienID == NhanVienID
                         select new
                         {
                             TuyenID = r.TuyenKHID,
                             Ten = r.Ten,
                             Matuyen = r.Matuyen,
                         }).Distinct().ToList();

            return Json(tuyen, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách tuyến theo nhân viên
        /// </summary>
        /// <param name="NhanVienID"></param>
        /// <returns></returns>
        public JsonResult FillNhanVienByTo(int? ToID)
        {
            var tuyen = (from i in db.Nhanviens
                         where i.ToQuanHuyenID == ToID.Value && i.IsDelete == false
                         select new
                         {
                             NhanvienID = i.NhanvienID,
                             Ten = i.Ten,
                             MaNhanVien = i.MaNhanVien,
                         }).Distinct().ToList();

            return Json(tuyen, JsonRequestBehavior.AllowGet);
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
            //Lấy Mã khách hàng lớn nhất trong hệ thống
            var soLuongKH = db.Khachhangs.Count();
            //kiểm tra xem nếu trong bảng khách hàng có dữ liệu hay không
            if (soLuongKH > 0)
            {
                //lấy ID primary key của record cuối cùng
                var lastKhachHangID = db.Khachhangs.Max(p => p.KhachhangID);
                string maKhachHang = db.Khachhangs.Where(p => p.KhachhangID == lastKhachHangID).FirstOrDefault().MaKhachHang;
                //nếu mã khách hàng != null thì tự động tăng mã KH lên 1
                if (maKhachHang != null)
                {
                    //load mã khách hàng (tự động tăng 1) vào view
                    ViewBag.MaKH = int.Parse(maKhachHang) + 1;
                }
                else
                {
                    ViewBag.MaKH = 1;
                }
            }
            //nếu không có khách hàng trong CSDL
            else
            {
                ViewBag.MaKH = 1;
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
            ViewBag.CumdancuID = db.Cumdancus.Where(p => p.IsDelete == false).ToList();
            ViewBag.HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten");
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int) EApGia.DacBiet), "LoaiapgiaID", "Ten");
            ViewBag.LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten");
            ViewBag.PhuongxaID = db.Phuongxas.Where(p => p.QuanhuyenID == selectedQuanHuyenID && p.IsDelete == false).ToList();
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList(), "QuanhuyenID", "Ten");
            ViewBag.TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);

            return View();
        }

        // POST: /Khachhang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KhachhangID,Makhachhang,QuanhuyenID,PhuongxaID,CumdancuID,TuyenKHID,LoaiKHID,LoaiapgiaID,HinhthucttID,TuyenongkythuatID,Sotaikhoan,Masothue,Ngaykyhopdong,Tilephimoitruong,Soho,Ngayap,Ngayhetap,Sonhankhau,Ten,Diachi,Dienthoai,Ghichu,Sokhuvuc,Sohopdong,Tinhtrang,Diachithutien,IsDelete,TTDoc")] Khachhang khachhang, FormCollection form)
        {
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            int ChiSoDau = String.IsNullOrEmpty(form["ChiSoDau"]) ? 0 : Convert.ToInt32(form["ChiSoDau"]);

            if (ModelState.IsValid)
            {
                khachhang.Chisolapdat = ChiSoDau;
                //lấy thứ tự đọc
                int ttDoc = khachhang.TTDoc.Value;
                int tuyenID = khachhang.TuyenKHID.Value;
                int countCustomer = db.Khachhangs.Count(p => p.TTDoc == ttDoc && (p.IsDelete == false || p.IsDelete == null) && p.TuyenKHID == khachhang.TuyenKHID);
                if (countCustomer > 0)
                {
                    khachHangHelper.pushKhachHangXuong(ttDoc, tuyenID);
                }
                //đặt tình trạng đang sử dụng
                khachhang.Tinhtrang = 0;
                db.Khachhangs.Add(khachhang);
                // lưu thay đổi vào DB và bắt ngoại lệ để debug            
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Response.Write(validationError.PropertyName + "--" + validationError.ErrorMessage);
                            Response.End();
                        }
                    }
                }
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

                //số hóa đơn, kí hiệu
                //khi tạo mới thì tổng số tiêu thụ = 0
                hoaDonNuoc.Tongsotieuthu = 0;
                //trạng thái in & trạng thái thu = false (chưa in và chưa thu)
                hoaDonNuoc.Trangthaiin = false;
                hoaDonNuoc.Trangthaithu = false;
                hoaDonNuoc.NamHoaDon = DateTime.Now.Year;
                hoaDonNuoc.ThangHoaDon = DateTime.Now.Month;
                db.Hoadonnuocs.Add(hoaDonNuoc);
                //Bắt lỗi validation khi lưu
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Response.Write(validationError.PropertyName + "--" + validationError.ErrorMessage);
                            Response.End();
                        }
                    }
                }
                /*------------------CHI TIẾT HÓA ĐƠN NƯƠC tại tháng hiện tại------------*/
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
                try
                {
                    db.SaveChanges();
                    ViewBag.successfulMessage = "Thêm mới khách hàng thành công";
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

            //Lấy Mã khách hàng lớn nhất trong hệ thống
            var soLuongKH = db.Khachhangs.Count();
            //kiểm tra xem nếu trong bảng khách hàng có dữ liệu hay không
            if (soLuongKH > 0)
            {
                //lấy ID primary key của record cuối cùng
                var lastKhachHangID = db.Khachhangs.Max(p => p.KhachhangID);
                string maKhachHang = db.Khachhangs.Where(p => p.KhachhangID == lastKhachHangID).FirstOrDefault().MaKhachHang;
                //nếu mã khách hàng != null thì tự động tăng mã KH lên 1
                if (maKhachHang != null)
                {
                    //load mã khách hàng (tự động tăng 1) vào view
                    ViewBag.MaKH = int.Parse(maKhachHang) + 1;
                }
                else
                {
                    ViewBag.MaKH = 1;
                }
            }
            //nếu không có khách hàng trong CSDL
            else
            {
                ViewBag.MaKH = 1;
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

            ViewBag.CumdancuID = db.Cumdancus.Where(p => p.IsDelete == false).ToList();
            ViewBag.selectedCumDanCu = khachhang.CumdancuID;
            ViewBag.HinhthucttID = new SelectList(db.Hinhthucthanhtoans, "HinhthucttID", "Ten", khachhang.HinhthucttID);
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p => p.LoaiapgiaID != (int)EApGia.DacBiet), "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
            ViewBag.LoaiKHID = new SelectList(db.LoaiKHs, "LoaiKHID", "Ten", khachhang.LoaiKHID);
            ViewBag.PhuongxaID = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedPhuongXa = khachhang.PhuongxaID;
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false), "QuanhuyenID", "Ten", khachhang.QuanhuyenID);
            ViewBag.TuyenongkythuatID = db.Tuyenongs.Where(p => p.IsDelete == false);
            ViewBag.selectedTuyenKHID = khachhang.TuyenKHID;
            ViewBag.reEnterCustomer = true;
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
            //ViewBag.Ngaykyhopdong = String.Format("{0:dd/mm/yyyy}", khachhang.Ngaykyhopdong);
            //ViewBag.NgayApDinh = String.Format("{0:dd/mm/yyyy}", khachhang.Ngayap);
            //ViewBag.NgayHetDinh = String.Format("{0:dd/mm/yyyy}", khachhang.Ngayhetap);
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
        public ActionResult Edit([Bind(Include = "KhachhangID,Makhachhang,QuanhuyenID,PhuongxaID,CumdancuID,TuyenKHID,LoaiKHID,LoaiapgiaID,HinhthucttID,TuyenongkythuatID,Sotaikhoan,Masothue,Ngaykyhopdong,Tilephimoitruong,Soho,Ngayap,Ngayhetap,Sonhankhau,Ten,Diachi,Dienthoai,Ghichu,Sokhuvuc,Sohopdong,Tinhtrang,Diachithutien,IsDelete,TTDoc")] Khachhang khachhang, FormCollection form, int? toID, int nhanVienIDUrl, int tuyenIDUrl)
        {
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
                if (chiSoDauEdited != -1)
                {
                    Hoadonnuoc hoaDonNuoc = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachhang.KhachhangID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year);
                    if (hoaDonNuoc != null)
                    {
                        Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == hoaDonNuoc.HoadonnuocID);
                        chiTiet.Chisocu = chiSoDauEdited;
                        db.Entry(chiTiet).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //xóa bảng áp giá tổng hơp trước
                List<Apgiatonghop> apTongHop = db.Apgiatonghops.Where(p => p.KhachhangID == khachhang.KhachhangID && p.ThangTongHop == DateTime.Now.Month && p.NamTongHop == DateTime.Now.Year).ToList();
                if (apTongHop != null)
                {
                    db.Apgiatonghops.RemoveRange(apTongHop);
                    db.SaveChanges();
                }
                //tiến hành áp giá và chia lại chỉ số
                int loaiApGiaID = Convert.ToInt32(form["LoaiapgiaID"]);
                int sanLuongTieuThu = 0; int chiSoDau = 0; int chiSoCuoi = 0; int soKhoan = 0;
                //lấy sản lượng đã có trong db
                Hoadonnuoc hD = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachhang.KhachhangID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year);
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
                        KhachHang.saveGiaTongHop(khachhang.KhachhangID, 1, SH, KD, HC, CC, SX, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                        sLTT.tachSoTongHop(hD.HoadonnuocID, 1, khachhang.KhachhangID, sanLuongTieuThu);
                    }
                    // tính theo chỉ số khoán
                    else
                    {
                        KhachHang.saveGiaTongHop(khachhang.KhachhangID, 0, SH, KD, HC, CC, SX, DateTime.Now.Month, Convert.ToInt16(DateTime.Now.Year));
                        sLTT.tachSoTongHop(hD.HoadonnuocID, 0, khachhang.KhachhangID, sanLuongTieuThu);
                    }
                    //chia lại giá                    
                }                
                //tách lại chỉ số giá khác
                else
                {
                    sLTT.tachChiSoSanLuong(hD.HoadonnuocID, chiSoDau, chiSoCuoi, sanLuongTieuThu, soKhoan, khachhang.KhachhangID);
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
            ViewBag._LoaiapgiaID = new SelectList(db.Loaiapgias.Where(p=>p.LoaiapgiaID!=(int)EApGia.DacBiet), "LoaiapgiaID", "Ten", khachhang.LoaiapgiaID);
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

        public ActionResult Inactive(int id, int toID, int nhanvienID, int tuyenID)
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
        public ActionResult Inactive(FormCollection form, int id, int toID, int nhanvienID, int tuyenID)
        {
            String thanhLy = form["Lydothanhly"];
            string[] hiddenKhachHang = form["thanhLy"].ToString().Split(',');
            foreach (var item in hiddenKhachHang)
            {
                int khachHangThanhLyID = Convert.ToInt32(item);
                //bằng 1 là đã thanh lý hơp đồng
                var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == khachHangThanhLyID);
                khachHang.Tinhtrang = 1;
                khachHang.Ngaythanhly = Convert.ToDateTime(form["ngayThanhLy"]);
                khachHang.Lydothanhly = thanhLy;
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                //cập nhật lại trạng thái của hóa đơn, từ hiển thị => xóa
                List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangThanhLyID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year).ToList();
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
                ViewBag.kiemDinh = db.Kiemdinhs.Where(p => p.KhachhangID == id).OrderByDescending(p => p.Ngaykiemdinh).ToList();
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
                                       //tách số
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
        public ActionResult CatNuoc(FormCollection form, int toID, int nhanvienID, int tuyenID)
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
                List<Hoadonnuoc> hoadons = db.Hoadonnuocs.Where(p => p.KhachhangID == khachHangNgungCapNuocID && p.ThangHoaDon == DateTime.Now.Month && p.NamHoaDon == DateTime.Now.Year).ToList();
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
        public ActionResult Caplainuoc(FormCollection form, int toID, int nhanvienID, int tuyenID)
        {
            String lyDoCapNuocLai = form["Lydocapnuoclai"];
            string[] hiddenKhachHang = form["capnuoclai"].ToString().Split(',');
            DateTime ngayCapNuocLai = Convert.ToDateTime(form["Ngaycapnuoclai"]);
            foreach (var item in hiddenKhachHang)
            {
                int khachHangNgungCapNuocID = Convert.ToInt32(item);
                //bằng 1 là đã thanh lý hơp đồng
                var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == khachHangNgungCapNuocID);
                khachHang.Ngayngungcapnuoc = null;
                khachHang.Lydongungcapnuoc = null;
                khachHang.Ngaycapnuoclai = ngayCapNuocLai;
                khachHang.Lydocapnuoclai = lyDoCapNuocLai;
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                //cập nhật lại trạng thái của hóa đơn, từ hiển thị => xóa
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
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int phongBanID = Convert.ToInt32(Session["phongBan"]);
            int nhanVienID = String.IsNullOrEmpty(form["nhanvien"]) ? 0 : Convert.ToInt32(form["nhanvien"]);

            int toForm = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            String nhanVien = form["nhanvien"];
            String tuyen = form["tuyen"];
            String maKH = String.IsNullOrEmpty(form["maKH"]) ? "" : form["maKH"];
            var khachHang = db.Khachhangs.Where(p => p.MaKhachHang == maKH).ToList();

            ViewBag.to = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.nhanVien = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == toForm && (p.IsDelete == false || p.IsDelete == null) && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
            var tuyenTheoNhanVien = from i in db.Tuyentheonhanviens
                                    join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                    join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                    join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                    where i.NhanVienID == nhanVienID
                                    select r;
            ViewBag.tuyen = tuyenTheoNhanVien.ToList();
            ViewBag.showKhachHang = true;
            ViewBag.khachHang = khachHang;
            return View("Index");
        }
    }
}