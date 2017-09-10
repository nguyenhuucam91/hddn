using HDNHD.Models.Constants;
using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Models.SoLieuTieuThu;
using HoaDonNuocHaDong.Repositories;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class SoLieuTieuThuController : BaseController
    {
        private ChiSo cS = new ChiSo();
        private Tuyen tuyenHelper = new Tuyen();
        private KhachHang kHHelper = new KhachHang();
        private NguoidungHelper ngDungHelper = new NguoidungHelper();
        private KiemDinh kiemDinh = new KiemDinh();
        private LichSuHoaDonRepository lichSuHoaDonRepository = new LichSuHoaDonRepository();

        private List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> hoaDons;

        // GET: /SoLieuTieuThu/
        /// <summary>
        /// Hiển thị danh sách chi nhánh, tổ, nhân viên, tuyến và khách hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? to, int? nhanvien, int? tuyen, int? thang, int? nam)
        {
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

            List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.showHoaDon = false;
            //load danh sách nhân viên thuộc tổ có phòng ban đó.
            if (nhanvien == null)
            {
                List<Nhanvien> nVLs = new List<Nhanvien>();
                ViewBag.nhanVien = nVLs;
            }
            else
            {
                List<Nhanvien> _nvLs = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == to && p.IsDelete == false).ToList();
                ViewBag.nhanVien = _nvLs;
            }
            //load danh sách tuyến thuộc nhân viên đó.
            if (tuyen == null)
            {
                List<Tuyenkhachhang> tuyensLs = new List<Tuyenkhachhang>();
                ViewBag.tuyen = tuyensLs;

            }
            else
            {
                bool isTruongPhong = ngDungHelper.isNguoiDungLaTruongPhong(nhanvien);
                if (isTruongPhong)
                {
                    List<HoaDonNuocHaDong.Models.ModelNhanVien> nhanviens = new List<HoaDonNuocHaDong.Models.ModelNhanVien>();
                    List<ToQuanHuyen> toes = getToes(quanHuyenID, phongBanID);
                    List<Tuyenkhachhang> tuyens = new List<Tuyenkhachhang>();
                    foreach (var toTruongPhong in toes)
                    {
                        nhanviens.AddRange(getNhanViensByTo(toTruongPhong.ToQuanHuyenID));
                    }

                    foreach (var nhanvienQuanLiTuyen in nhanviens)
                    {
                        List<Tuyenkhachhang> tuyensThuocNhanVien = tuyenHelper.getDanhSachTuyensByNhanVien(nhanvienQuanLiTuyen.NhanvienID).ToList();
                        tuyens.AddRange(tuyensThuocNhanVien);
                    }

                    ViewBag.tuyen = tuyens;
                }

                else
                {
                    List<Tuyenkhachhang> tuyensLs = new List<Tuyenkhachhang>();
                    var tuyenTheoNhanVien = (from i in db.Tuyentheonhanviens
                                             join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                             join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                             join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                             where i.NhanVienID == nhanvien && r.IsDelete == false
                                             orderby r.Matuyen
                                             select r).ToList();
                    tuyensLs.AddRange(tuyenTheoNhanVien);
                    ViewBag.tuyen = tuyensLs;
                }
                //lấy danh sách khách hàng thuộc tuyến đó
                List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cS.filterChiSo(thang.Value, nam.Value, tuyen.Value);
                ViewData["ChiSoTieuThu"] = chiSoTieuThu;
                ViewBag.showHoaDon = true;
                ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyen);
                Nhanvien nhanVienObj = db.Nhanviens.Find(nhanvien);
                ViewData["nhanVienObj"] = nhanVienObj;
                ViewBag.tongSoHoaDon = chiSoTieuThu.Count;
                ViewBag.showHoaDon = true;
                hoaDons = chiSoTieuThu;
            }

            #region ViewBag
            //load viewBag ngày bắt đầu            
            ViewBag.month = thang == null ? DateTime.Now.Month : thang;
            ViewBag.year = nam == null ? DateTime.Now.Year : nam;
            //kiểm đinh            
            ViewBag.ngayBatDau = "";
            ViewBag.ngayKetThuc = "";
            ViewBag.to = toLs;
            List<string> errors = new List<string>();
            ViewData["errorList"] = errors;
            ViewBag.selectedTo = to;
            ViewBag.selectedTuyen = tuyen;
            ViewBag.selectedNhanvien = nhanvien;
            ViewBag.selectedChiNhanh = quanHuyenID;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);

            #endregion
            return View();
        }

        /// <summary>
        /// Hàm để lọc chi tiết hóa đơn theo tháng và năm
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            #region parameters
            List<String> errorList = new List<String>();
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            int nhanVienInt = String.IsNullOrEmpty(form["nhanvien"]) ? 0 : Convert.ToInt32(form["nhanvien"]);
            //gán session cho nhân viên để tiến hành thêm mới hóa đơn tháng sau cũng như lấy nhân viên cho các controller khác có thể tác động vào.          
            Session["nhanvien"] = nhanVienInt;
            String month = form["thang"];
            String year = form["nam"];
            //nếu năm tháng rỗng thì lấy năm và tháng hiện tại, nếu tuyến được chọn rỗng thì lấy là 0
            int _month = String.IsNullOrEmpty(month) ? DateTime.Now.Month : Convert.ToInt16(month);
            int _year = String.IsNullOrEmpty(year) ? DateTime.Now.Year : Convert.ToInt16(year);
            String selectedNhanVien = form["nhanvien"];
            String selectedTuyen = form["tuyen"];
            int tuyenInt = Convert.ToInt32(selectedTuyen);
            //sao chép ds khách hàng không sản lượng vào tháng hiện tại                       
            ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenInt);
            #endregion

            if (new DateTime(_year, _month, 1) > new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            {
                errorList.Add("Ngày tháng được chọn không được quá ngày tháng hiện tại");
            }
            else
            {
                cS.generateChiSoFromNearestMonth(_month, _year, nhanVienInt, Convert.ToInt32(selectedTuyen));
            }
            //lấy danh sách tổ, phòng ban thuộc tổ quận huyện đó
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

            ViewBag.to = new List<ToQuanHuyen>();
            ViewBag.nhanVien = new List<Nhanvien>();
            ViewBag.tuyen = new List<Tuyenkhachhang>();

            int toForm = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            if (toForm == 0)
            {
                errorList.Add("Tổ không được để trống");
                List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
                ViewBag.to = toLs;
            }
            else
            {
                List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
                ViewBag.to = toLs;

                //load danh sách nhân viên thuộc tổ có phòng ban đó, lấy từ form được chọn
                List<Nhanvien> _nvLs = db.Nhanviens.Where(p => p.ToQuanHuyenID == toForm && p.PhongbanID == phongBanID && p.IsDelete == false).ToList();
                ViewBag.nhanVien = _nvLs;
                //nếu tổ không rỗng thì kiểm tra nhân viên xem có rỗng hay không                
                if (String.IsNullOrEmpty(selectedNhanVien))
                {
                    errorList.Add("Nhân viên không được để trống");
                    ViewBag.nhanVien = new List<Nhanvien>();
                }
                //nếu nhân viên không để trống thì kiểm tra tuyến
                else
                {
                    bool isTruongPhong = ngDungHelper.isNguoiDungLaTruongPhong(nhanVienInt);
                    if (isTruongPhong)
                    {
                        List<HoaDonNuocHaDong.Models.ModelNhanVien> nhanviens = new List<HoaDonNuocHaDong.Models.ModelNhanVien>();
                        List<ToQuanHuyen> toes = getToes(quanHuyenID, phongBanID);
                        List<Tuyenkhachhang> tuyens = new List<Tuyenkhachhang>();
                        foreach (var to in toes)
                        {
                            nhanviens.AddRange(getNhanViensByTo(to.ToQuanHuyenID));
                        }

                        foreach (var nhanvien in nhanviens)
                        {
                            List<Tuyenkhachhang> tuyensThuocNhanVien = tuyenHelper.getDanhSachTuyensByNhanVien(nhanvien.NhanvienID).ToList();
                            tuyens.AddRange(tuyensThuocNhanVien);
                        }
                        ViewBag.tuyen = tuyens;
                    }
                    else
                    {
                        //load danh sách tuyến thuộc nhân viên đó.
                        List<Tuyenkhachhang> tuyensLs = new List<Tuyenkhachhang>();

                        var tuyenTheoNhanVien = (from i in db.Tuyentheonhanviens
                                                 join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                                 join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                                 join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                                 where i.NhanVienID == nhanVienInt
                                                 select r).ToList();
                        tuyensLs.AddRange(tuyenTheoNhanVien);
                        ViewBag.tuyen = tuyensLs;
                    }
                    Session["solieuTieuThuNhanvien"] = Convert.ToInt32(selectedNhanVien);
                    //nếu tuyến để trống thì thêm mới lỗi vào errorList
                    if (String.IsNullOrEmpty(selectedTuyen))
                    {
                        errorList.Add("Không nhập được số liệu tiêu thụ do tuyến để trống");
                    }
                    else
                    {
                        ViewBag.selectedNhanvien = Session["solieuTieuThuNhanvien"];
                        ViewBag.selectedTuyen = tuyenInt;
                        ViewBag.selectedTo = toForm;
                    }
                    Nhanvien nhanVienObj = db.Nhanviens.Find(nhanVienInt);
                    ViewData["nhanVienObj"] = nhanVienObj;
                    ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenInt);
                    List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cS.filterChiSo(_month, _year, tuyenInt);
                    ViewData["ChiSoTieuThu"] = chiSoTieuThu;
                    ViewBag.tongSoHoaDon = chiSoTieuThu.Count;
                }
            }

            int _selectedTuyen = String.IsNullOrEmpty(selectedTuyen) ? 0 : Convert.ToInt32(selectedTuyen);


            //kiểm tra xem nhân viên đó là trưởng phòng hay nhân viên, nếu là trưởng phòng thì cho chỉnh sửa thoải mái
            int nhanVienIDLoggedIn = LoggedInUser.NhanvienID.Value;
            int? chucVuID = db.Nhanviens.Find(nhanVienIDLoggedIn).ChucvuID;
            //nếu là nhân viên thì được cập nhật trạng thái chốt trong trường hợp chốt đúng
            if (chucVuID == null || chucVuID == ChucVuHelper.NHANVIEN)
            {
                //xem tuyến đã chốt chưa, nếu chốt rồi thì k cho chỉnh sửa nữa          
                TuyenDuocChot dcChot = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == _selectedTuyen && p.Thang == _month && p.Nam == _year);
                if (dcChot != null)
                {
                    if (dcChot.TrangThaiChot == true)
                    {
                        ViewBag.isChot = true;
                    }
                }
                else
                {
                    ViewBag.isChot = false;
                }
            }
            //nếu là trường phòng thì ko bao giờ chốt, cho edit thoải mái
            else
            {
                ViewBag.isChot = false;
            }
            #region ViewBag
            int loggedInRole = getUserRole(LoggedInUser.NhanvienID);
            //lấy danh sách quận huyện để đẩy vào phần lọc chỉ số KH
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            //dành cho người dùng
            ViewBag.showHoaDon = true;
            ViewBag.month = _month;
            ViewBag.year = _year;
            ViewBag.isAdminVaTruongPhong = loggedInRole;
            ViewBag.nextMonth = "";
            ViewData["errorList"] = errorList;
            #endregion

            return View();
        }



        /// <summary>
        /// Hàm để lưu dữ liệu thông số chỉ số vào hệ thống
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="ChiSoDau"></param>
        /// <param name="ChiSoCuoi"></param>
        /// <param name="TongSoTieuThu"></param>
        /// <param name="hieuSo"></param>
        public void NhapChiSoMoi(int HoaDonID, int? ChiSoDau, int? ChiSoCuoi, int? TongSoTieuThu, int SoKhoan, int KHID,
            int SoHoaDon, String dateStart, String dateEnd, String dateInput, int thang, int nam)
        {
            int _month = thang;
            int _year = nam;
            int _TongSoTieuThu = 0;
            int _tongKiemDinh = 0;
            int nhanVienId = 0;
            //HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc hoaDon = hoaDons.FirstOrDefault(p => p.HoaDonNuocID == HoaDonID);
            //nếu có record hóa đơn trọng hệ thống

            if (LoggedInUser.NhanvienID != null)
            {
                nhanVienId = LoggedInUser.NhanvienID.Value;
                //hoaDon.nhanVienId = LoggedInUser.NhanvienID.Value;
            }

            var isKiemDinh = kiemDinh.checkKiemDinhStatus(KHID, _month, _year);
            //nếu khách hàng đang chỉnh sửa chưa kiểm định mà nhập số thì tính như bình thường
            if (isKiemDinh)
            {
                var kiemDinh1 = kiemDinh.getChiSoLucKiemDinh(KHID, _month, _year) - ChiSoDau.Value;
                var kiemDinh2 = ChiSoCuoi.Value - kiemDinh.getChiSoSauKiemDinh(KHID, _month, _year);
                _tongKiemDinh = kiemDinh1 + kiemDinh2;
                _TongSoTieuThu = _tongKiemDinh;
            }
            else
            {
                _TongSoTieuThu = TongSoTieuThu.Value;
            }

            //hoaDon.NgayBatDauSuDung = Convert.ToDateTime(dateStart);
            //hoaDon.NgayKetThucSuDung = Convert.ToDateTime(dateEnd);
            //hoaDon.SanLuong = _TongSoTieuThu;
            //db.SaveChanges();



            using (SqlConnection con = new SqlConnection(HoaDonNuocHaDong.Config.DatabaseConfig.getConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("NhapChiSoTieuThuThang", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nhanVienId", nhanVienId);
                    cmd.Parameters.AddWithValue("@ngayBatDauSuDung", Convert.ToDateTime(dateStart));
                    cmd.Parameters.AddWithValue("@ngayKetThucSuDung", Convert.ToDateTime(dateEnd));
                    cmd.Parameters.AddWithValue("@sanLuong", _TongSoTieuThu);
                    cmd.Parameters.AddWithValue("@hoaDonId", HoaDonID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            tachChiSoSanLuong(HoaDonID, ChiSoDau.Value, ChiSoCuoi.Value, _TongSoTieuThu, SoKhoan, KHID);
            HoaDonNuocHaDong.Helper.HoaDonNuoc.themMoiHoaDonThangSau(KHID, HoaDonID, ChiSoCuoi.Value, LoggedInUser.NhanvienID.Value, _month, _year, Convert.ToDateTime(dateEnd));

            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);
            Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);

            //tongTienHoaDon;
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Math.Round(dinhMuc * 0.05, 0, MidpointRounding.AwayFromZero);
            double thueBVMT = cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value);
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
            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - " + SoHoaDon;
            //cộng dồn
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc);
            double congDonHDTruoc = 0;
            if (count == 0)
            {
                congDonHDTruoc = 0;
            }
            else
            {
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc.Value < obj.TTDoc.Value).Sum(p => p.TongCong.Value);
            }
            double tongCongCongDon = Convert.ToDouble(tongTienHoaDon + congDonHDTruoc);


            lichSuHoaDonRepository.updateLichSuHoaDon(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, _TongSoTieuThu,
            cT.SH1.Value, cS.getSoTienTheoApGia("SH1").Value,
            cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value,
            cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value,
            cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
            cT.HC.Value, cS.getSoTienTheoApGia("HC").Value,
            cT.CC.Value, cS.getSoTienTheoApGia("CC").Value,
            cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value,
            cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
            dinhMuc,
            5, VAT,
            obj.Tilephimoitruong.Value, thueBVMT, tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
            db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
            thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, dateStart, dateEnd);

            themMoiSoTienPhaiNop(HoaDonID);
        }

        public void tachChiSoSanLuong(int HoaDonID, int ChiSoDau, int ChiSoCuoi, int TongSoTieuThu, int SoKhoan, int KHID)
        {
            Hoadonnuoc hoaDon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            List<Apgia> apGia = new List<Apgia>();
            //cập nhật vào bảng chi tiết hóa đơn nước và bảng hóa đơn nươc            
            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            //lấy giá áp hiện tại cho khách hàng này
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            int _loaiApGia = 0;
            //Tổng số tiêu thụ  = chỉ số cuối - chỉ Số đầu + Số Khoán (có thể âm dương)
            //kiểm tra kiểm định

            int _TongSoTieuThu = TongSoTieuThu;
            if (khachHang != null)
            {
                //lấy loại áp giá cho khách hàng có ID = KHID
                _loaiApGia = khachHang.LoaiapgiaID.Value;
                apGia = db.Apgias.Where(p => p.LoaiapgiaID == _loaiApGia).ToList();
            }
            //cập nhật bảng chi tiết
            if (hoaDon != null)
            {
                chiTiet.HoadonnuocID = hoaDon.HoadonnuocID;
                chiTiet.Chisocu = ChiSoDau;
                chiTiet.Chisomoi = ChiSoCuoi;
                //tách số (xem lại)
                if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.SINHHOAT)
                {
                    int soNhanKhau = kHHelper.getSoNhanKhau(KHID);
                    int soHo = kHHelper.getSoHo(KHID);
                    int dinhMucCoSo = 10;
                    double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);

                    double SH1, SH2, SH3, SH4;
                    SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;

                    double dinhMucSH1 = dinhMucTungNha;
                    double dinhMucSH2 = dinhMucTungNha * 2;
                    double dinhMucSH3 = dinhMucTungNha * 3;


                    if (_TongSoTieuThu - SH1 <= 0)
                    {
                        SH2 = 0;
                    }
                    else
                    {
                        //22
                        //SH2 = 16 <=10 ? 6 : 10 ;
                        SH2 = _TongSoTieuThu - SH1 <= (dinhMucSH2 - dinhMucSH1) ? _TongSoTieuThu - SH1 : (dinhMucSH2 - dinhMucSH1);
                    }

                    if (_TongSoTieuThu - SH1 - SH2 <= 0)
                    {
                        SH3 = 0;
                    }
                    else
                    {
                        SH3 = _TongSoTieuThu - SH1 - SH2 <= (dinhMucSH3 - dinhMucSH2) ? _TongSoTieuThu - SH1 - SH2 : (dinhMucSH3 - dinhMucSH2);
                    }

                    if (_TongSoTieuThu - SH1 - SH2 - SH3 <= 0)
                    {
                        SH4 = 0;
                    }
                    else
                    {
                        SH4 = _TongSoTieuThu - SH1 - SH2 - SH3;
                    }

                    chiTiet.SH1 = SH1;
                    chiTiet.SH2 = SH2;
                    chiTiet.SH3 = SH3;
                    chiTiet.SH4 = SH4;
                    chiTiet.CC = 0;
                    chiTiet.HC = 0;
                    chiTiet.KDDV = 0;
                    chiTiet.SXXD = 0;
                }
                //tách số tổng hợp
                else if (_loaiApGia == KhachHang.TONGHOP)
                {
                    int cachTinh = db.Apgiatonghops.FirstOrDefault(p => p.KhachhangID == KHID).CachTinh.Value;
                    //lấy các chỉ số liên quan đến áp giá tổng hợp
                    tachSoTongHop(HoaDonID, cachTinh, KHID, TongSoTieuThu);
                }
                else if (_loaiApGia == KhachHang.DACBIET)
                {
                    tachSoTongHop(HoaDonID, -1, KHID, TongSoTieuThu);
                }
                //loại khách hàng: doanh nghiệp - tính theo số kinh doanh, CC,...
                else
                {
                    chiTiet.SH1 = 0;
                    chiTiet.SH2 = 0;
                    chiTiet.SH3 = 0;
                    chiTiet.SH4 = 0;
                    chiTiet.CC = 0;
                    chiTiet.HC = 0;
                    chiTiet.SXXD = 0;
                    chiTiet.KDDV = 0;
                    //nếu khách hàng là kinh doanh 
                    if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.KINHDOANHDICHVU)
                    {
                        chiTiet.KDDV = _TongSoTieuThu;
                    }
                    //Sản xuất
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.SANXUAT)
                    {
                        chiTiet.SXXD = _TongSoTieuThu;
                    }
                    //đơn vị sự nghiệp
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.DONVISUNGHIEP)
                    {
                        chiTiet.CC = _TongSoTieuThu;
                    }
                    //kinh doanh dịch vụ
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.COQUANHANHCHINH)
                    {
                        chiTiet.HC = _TongSoTieuThu;
                    }
                }
                db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Chỉnh sửa số khoán để cập nhật vào db
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="SoKhoan"></param>
        /// <param name="KHID"></param>
        public void ChinhSuaSoKhoan(int HoaDonID, int? ChiSoDau, int? ChiSoCuoi, int? TongSoTieuThu, int SoKhoan, int KHID, String dateStart, String dateEnd, String sohoadon)
        {
            Hoadonnuoc hoaDon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            int _month = hoaDon.ThangHoaDon.Value;
            int _year = hoaDon.NamHoaDon.Value;
            hoaDon.SoKhoan = SoKhoan;
            hoaDon.Tongsotieuthu = TongSoTieuThu;
            db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Cập nhật lại SH1...
            tachChiSoSanLuong(HoaDonID, ChiSoDau.Value, ChiSoCuoi.Value, TongSoTieuThu.Value, SoKhoan, KHID);
            HoaDonNuocHaDong.Helper.HoaDonNuoc.themMoiHoaDonThangSau(KHID, HoaDonID, ChiSoCuoi.Value, Convert.ToInt32(Session["nhanvien"]), _month, _year, Convert.ToDateTime(dateEnd));
            //thêm vào bảng lịch sử sử dụng nước
            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);

            Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            //tongTienHoaDon;
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
            double thueBVMT = cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value);
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

            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - " + sohoadon;

            //tổng cộng dồn
            double congDonHDTruoc = 0;
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc);
            if (count == 0)
            {
                congDonHDTruoc = 0;
            }
            else
            {
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc.Value).Sum(p => p.TongCong.Value);
            }
            double tongCongCongDon = tongTienHoaDon + congDonHDTruoc;

            lichSuHoaDonRepository.updateLichSuHoaDon(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, TongSoTieuThu.Value, cT.SH1.Value,
            cS.getSoTienTheoApGia("SH1").Value, cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value, cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value, cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
            cT.HC.Value, cS.getSoTienTheoApGia("HC").Value, cT.CC.Value, cS.getSoTienTheoApGia("CC").Value, cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value, cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
            dinhMuc, 5, VAT,
            obj.Tilephimoitruong.Value, thueBVMT,
            tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
            db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3, thuNgan, obj.TuyenKHID.Value,
            obj.TTDoc.Value, tongCongCongDon, dateStart, dateEnd);
            themMoiSoTienPhaiNop(HoaDonID);
        }

        /// <summary>
        /// Xem chi tiết hóa đơn
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewDetail(String month, String year, int? tuyenID, int? nvquanly)
        {
            int _month, _year;
            if (String.IsNullOrEmpty(month))
            {
                _month = DateTime.Now.Month;
            }
            else
            {
                _month = Convert.ToInt16(month);
            }

            if (String.IsNullOrEmpty(year))
            {
                _year = DateTime.Now.Year;
            }
            else
            {
                _year = Convert.ToInt16(year);
            }

            //load danh sách khách hàng thuộc tuyến mà người dùng (nhân viên) đăng nhập                       
            ViewBag.tenTuyen = UserInfo.getTenTuyen(tuyenID.Value);
            //load chỉ số và thông tin tách số vào đây
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = getDanhSachHoaDonTieuThu(_month, _year, tuyenID.Value);
            int soLuongHoaDonChuaChot = chiSoTieuThu.Count(p => p.TrangThaiChot == "False");
            int loggedInRole = getUserRole(LoggedInUser.NhanvienID);

            #region ViewBag
            ViewBag.trangthaiChotTuyen = soLuongHoaDonChuaChot;
            ViewBag.chiSoTieuThu = chiSoTieuThu;
            ViewBag.isAdminHoacTruongPhong = loggedInRole;
            ViewBag.soLuongHoaDonCoSanLuong = chiSoTieuThu.Count(p => p.SanLuong > 1);
            ViewBag.soLuongHoaDonKhongCoSanLuong = chiSoTieuThu.Count(p => p.SanLuong <= 1);
            ViewBag.soLuongHoaDon = chiSoTieuThu.Count();
            ViewData["nhanvien"] = db.Nhanviens.Find(nvquanly);
            ViewData["tuyen"] = db.Tuyenkhachhangs.Find(tuyenID);
            #endregion
            return View();
        }



        public List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> getDanhSachHoaDonTieuThu(int _month, int _year, int tuyenID)
        {
            ControllerBase<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> cB = new ControllerBase<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc>();
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cB.Query("ChiaChiSoTieuThuKhachHang", new SqlParameter("@month", _month), new SqlParameter("@year", _year),
            new SqlParameter("@tuyen", tuyenID)).ToList();
            return chiSoTieuThu;
        }


        /// <summary>
        /// Xem thông tin chi tiết phần kiểm định thuộc khách hàng nào đó
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public JsonResult showDetail(int KhachHangID, int month, int year)
        {
            var kiemDinh = from i in db.Kiemdinhs
                           join h in db.Hoadonnuocs on i.HoaDonId equals h.HoadonnuocID
                           where i.KhachhangID == KhachHangID && h.ThangHoaDon == month && h.NamHoaDon == year
                           select new
                           {
                               NgayKiemDinh = i.Ngaykiemdinh.Value.Day,
                               ThangKiemDinh = i.Ngaykiemdinh.Value.Month,
                               NamKiemDinh = i.Ngaykiemdinh.Value.Year,
                               GhiChu = i.Ghichu,
                               ChiSoLucKiemDinh = i.Chisoluckiemdinh,
                               ChiSoSauKiemDinh = i.Chisosaukiemdinh
                           };
            return Json(kiemDinh, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Áp giá đặc biệt cho khách hàng
        /// </summary>
        /// <param name="id">Hoa Dơn id</param>
        /// <returns></returns>
        public ActionResult Apgiadacbiet(int? id, int month, int year, int sohoadon)
        {
            var hoaDonNuoc = db.Hoadonnuocs.Join(db.Khachhangs, p => p.KhachhangID, q => q.KhachhangID, (p, q) => new { KhachHang = q, HoaDonNuoc = p })
                .FirstOrDefault(p => p.HoaDonNuoc.HoadonnuocID == id.Value);

            if (hoaDonNuoc != null)
            {
                ViewBag.maKH = hoaDonNuoc.KhachHang.MaKhachHang;
                ViewBag.tenKH = hoaDonNuoc.KhachHang.Ten;
                ViewBag.tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == hoaDonNuoc.KhachHang.TuyenKHID).Ten;
                ViewBag.maSoThue = hoaDonNuoc.KhachHang.Masothue;
                ViewBag.month = month;
                ViewBag.year = year;
                ViewBag.sanLuong = Convert.ToInt32(Request.QueryString["chisotieuthu"]);
                ViewBag.sohoadon = sohoadon;
                //load áp giá       
                ApGiaDacBiet giaDacBiet = db.ApGiaDacBiets.FirstOrDefault(p => p.HoaDonNuocID == id);
                if (giaDacBiet != null)
                {
                    ViewData["giaDacBiet"] = giaDacBiet;
                }
                else
                {
                    ViewData["giaDacBiet"] = new ApGiaDacBiet();
                }
            }
            else
            {
                return HttpNotFound("Không tìm thấy");
            }

            //tìm trong bảng lịch sử hóa đơn xem có record đó chưa, nếu có rồi thì load vào
            var lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == id && p.ThangHoaDon == month && p.NamHoaDon == year);
            var loaiApGia = db.Loaiapgias.FirstOrDefault(p => p.LoaiapgiaID == hoaDonNuoc.KhachHang.LoaiapgiaID);
            String tenApgia = loaiApGia != null ? loaiApGia.Ten : "Không có";
            ViewBag.tenApGia = tenApgia;
            if (lichSuHoaDon != null)
            {
                ViewBag.startDate = lichSuHoaDon.NgayBatDau;
                ViewBag.endDate = lichSuHoaDon.NgayKetThuc;
            }
            ViewBag.id = id.Value;
            return View();
        }

        /// <summary>
        /// Nhập giá đặc biệt cho khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Nhapgiadacbiet(int? id, FormCollection form, int? to, int nhanvien, int tuyen, int thang, int nam)
        {
            #region chiSoGiaDacBietChiTiet
            double SH1 = ChiSo.checkChiSoNull(form["SH1"]);
            double SH2 = ChiSo.checkChiSoNull(form["SH2"]);
            double SH3 = ChiSo.checkChiSoNull(form["SH3"]);
            double SH4 = ChiSo.checkChiSoNull(form["SH4"]);
            double HC = ChiSo.checkChiSoNull(form["HC"]);
            double CC = ChiSo.checkChiSoNull(form["CC"]);
            double SX = ChiSo.checkChiSoNull(form["SX"]);
            double KD = ChiSo.checkChiSoNull(form["KD"]);
            String startDate = form["startDateApGiaDacBiet"];
            String endDate = form["endDateApGiaDacBiet"];
            String soHoaDon = form["soHoaDon"];

            int Sum = Convert.ToInt32(SH1 + SH2 + SH3 + SH4 + HC + CC + SX + KD);
            #endregion
            ApGiaDacBiet apGiaDacBiet = db.ApGiaDacBiets.FirstOrDefault(p => p.HoaDonNuocID == id);
            if (apGiaDacBiet == null)
            {
                ApGiaDacBiet _apGia = new ApGiaDacBiet();
                _apGia.HoaDonNuocID = id;
                _apGia.KDDV = KD;
                _apGia.SH1 = SH1;
                _apGia.SH2 = SH2;
                _apGia.SH3 = SH3;
                _apGia.SH4 = SH4;
                _apGia.HC = HC;
                _apGia.CC = CC;
                _apGia.SXSD = SX;
                db.ApGiaDacBiets.Add(_apGia);
                db.SaveChanges();
            }
            else
            {
                apGiaDacBiet.HoaDonNuocID = id;
                apGiaDacBiet.KDDV = KD;
                apGiaDacBiet.SH1 = SH1;
                apGiaDacBiet.SH2 = SH2;
                apGiaDacBiet.SH3 = SH3;
                apGiaDacBiet.SH4 = SH4;
                apGiaDacBiet.HC = HC;
                apGiaDacBiet.CC = CC;
                apGiaDacBiet.SXSD = SX;
                db.Entry(apGiaDacBiet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            int ChiSoCu = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == id).Chisocu.Value;
            //thay đổi sản lượng và chỉ số mới của hóa đơn tháng đó
            Hoadonnuoc hoaDonDacBiet = db.Hoadonnuocs.Find(id);
            hoaDonDacBiet.Tongsotieuthu = Sum;
            db.Entry(hoaDonDacBiet).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //rồi cập nhật lại chỉ số của tháng đó
            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == id);
            chiTiet.Chisomoi = ChiSoCu + Sum;
            chiTiet.KDDV = KD;
            chiTiet.SH1 = SH1;
            chiTiet.SH2 = SH2;
            chiTiet.SH3 = SH3;
            chiTiet.SH4 = SH4;
            chiTiet.CC = CC;
            chiTiet.HC = HC;
            chiTiet.SXXD = SX;
            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //thêm mới hóa đơn nước và chi tiết hóa đơn nước của tháng sau
            int KHID = db.Hoadonnuocs.Find(id).KhachhangID.Value;
            //thêm 1 records số tiền phải nộp
            //HoaDonNuoc.themMoiHoaDonThangSau(KHID, id.Value, ChiSoCu + Sum, Convert.ToInt32(Session["nhanvien"]),null,null);

            //thêm vào bảng lịch sử
            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);

            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);
            int _month = hoaDonDacBiet.ThangHoaDon.Value;
            int _year = hoaDonDacBiet.NamHoaDon.Value;
            int ChiSoDau = chiTiet.Chisocu.Value;
            int ChiSoCuoi = chiTiet.Chisomoi.Value;
            //Giá 
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(id.Value, chiTiet.SH1.Value, chiTiet.SH2.Value, chiTiet.SH3.Value, chiTiet.SH4.Value, chiTiet.HC.Value, chiTiet.CC.Value, chiTiet.KDDV.Value, chiTiet.SXXD.Value);
            double thueBVMT = cS.tinhThue(id.Value, chiTiet.SH1.Value, chiTiet.SH2.Value, chiTiet.SH3.Value, chiTiet.SH4.Value, chiTiet.HC.Value, chiTiet.CC.Value, chiTiet.KDDV.Value, chiTiet.SXXD.Value, obj.Tilephimoitruong.Value);
            double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
            //chỉ số cộng dồn
            double tongTienHoaDon = dinhMuc + thueBVMT + VAT;
            double congDonHDTruoc = 0;
            //kiểm tra, nếu là record đầu tiên
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc);
            if (count == 0)
            {
                congDonHDTruoc = 0;
            }
            else
            {
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc.Value < obj.TTDoc.Value).Sum(p => p.TongCong.Value);
            }
            double tongCongCongDon = tongTienHoaDon + congDonHDTruoc;
            //thu ngân
            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon;

            lichSuHoaDonRepository.updateLichSuHoaDon(id.Value, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau, ChiSoCuoi, Sum,
              SH1, cS.getSoTienTheoApGia("SH1").Value,
              SH2, cS.getSoTienTheoApGia("SH2").Value,
              SH3, cS.getSoTienTheoApGia("SH3").Value,
              SH4, cS.getSoTienTheoApGia("SH4").Value,
              HC, cS.getSoTienTheoApGia("HC").Value,
              CC, cS.getSoTienTheoApGia("CC").Value,
              SX, cS.getSoTienTheoApGia("SX-XD").Value,
              KD, cS.getSoTienTheoApGia("KDDV").Value,
              dinhMuc,
              5, VAT, obj.Tilephimoitruong.Value, thueBVMT,
              tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
              db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
              thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, startDate, endDate);
            themMoiSoTienPhaiNop(id.Value);

            return RedirectToAction("Index", new { to = to, nhanvien = nhanvien, tuyen = tuyen, thang = thang, nam = nam });
        }

        /// <summary>
        /// Thêm mới số tiền phải nộp trong tháng đó
        /// </summary>
        /// <param name="HoaDonID"></param>

        public void themMoiSoTienPhaiNop(int HoaDonID)
        {
            var lichSuHD = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == HoaDonID);
            var hoaDon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);

            if (lichSuHD == null || hoaDon == null)
            {
                throw new Exception("SoLieuTieuThuController.themMoiSoTienPhaiNop: lichSuHoaDon & hoaDon are required.");
            }

            /* congnv 170515 */
            var stntt = db.SoTienNopTheoThangs.FirstOrDefault(p => p.HoaDonNuocID == HoaDonID);
            var ducoTruoc = db.DuCoes.FirstOrDefault(m => m.KhachhangID == hoaDon.KhachhangID && m.TienNopTheoThangID < hoaDon.SoTienNopTheoThangID);
            //var ducoTruoc = db.DuCoes.FirstOrDefault(m => m.KhachhangID == hoaDon.KhachhangID // dư có trước đó chưa trừ hết
            //    && (
            //        m.TrangThaiTruHet == false || // chưa trừ hoặc đã trừ hết cho hóa đơn này trước đó (trong TH cập nhật)
            //        (m.TrangThaiTruHet == true && m.NgayTruHet.Value.Month == hoaDon.ThangHoaDon && m.NgayTruHet.Value.Year == hoaDon.NamHoaDon)
            //    )
            //);
            DuCo duCo = null; // dư có tháng này (trong TH cập nhật)

            if (stntt == null)
            {
                stntt = new SoTienNopTheoThang()
                {
                    HoaDonNuocID = HoaDonID,
                    SoTienDaThu = 0,
                    SoTienPhaiNop = 0
                };

                db.SoTienNopTheoThangs.Add(stntt);
            }
            else
            {
                // reset hoaDon status
                hoaDon.Trangthaithu = false;
                hoaDon.NgayNopTien = null;

                duCo = db.DuCoes.FirstOrDefault(m => m.TienNopTheoThangID == stntt.ID);
            }

            stntt.SoTienTrenHoaDon = (int)lichSuHD.TongCong;
            stntt.SoTienPhaiNop = stntt.SoTienTrenHoaDon;

            if (ducoTruoc != null) // trừ dư có (nếu có)
            {
                ducoTruoc.TrangThaiTruHet = true;
                ducoTruoc.NgayTruHet = new DateTime(hoaDon.NamHoaDon.Value, hoaDon.ThangHoaDon.Value, 1);

                if (ducoTruoc.SoTienDu <= stntt.SoTienTrenHoaDon)
                {
                    stntt.SoTienPhaiNop -= ducoTruoc.SoTienDu;

                    if (duCo != null)
                    {
                        db.DuCoes.Remove(duCo);
                    }
                }
                else
                {
                    stntt.SoTienPhaiNop = 0;

                    // save db cập nhật stntt.ID
                    db.SaveChanges();

                    if (duCo == null)
                    {
                        duCo = new DuCo()
                        {
                            KhachhangID = hoaDon.KhachhangID,
                            TienNopTheoThangID = stntt.ID,
                        };
                        db.DuCoes.Add(duCo);
                    }

                    duCo.SoTienDu = ducoTruoc.SoTienDu - stntt.SoTienTrenHoaDon;
                }
            }

            if (stntt.SoTienPhaiNop == 0)
            {
                // update hoadon
                hoaDon.Trangthaithu = true;
                hoaDon.NgayNopTien = new DateTime(hoaDon.NamHoaDon.Value, hoaDon.ThangHoaDon.Value, 1);
            }

            db.SaveChanges();
            /* END congnv 170515 */
        }

        public List<HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop> sortApGiaTongHopBasedOnPriority(List<Apgiatonghop> ls)
        {
            List<HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop> sorted = (from i in ls
                                                                       join r in db.Loaiapgias on i.IDLoaiApGia equals (byte)r.LoaiapgiaID
                                                                       where i.SanLuong != 0
                                                                       orderby r.MucDoUuTienTinhGia
                                                                       select new HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop
                                                                       {
                                                                           SanLuong = i.SanLuong.Value,
                                                                           IDLoaiApGia = i.IDLoaiApGia.Value
                                                                       }).ToList();

            List<HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop> sanLuongZero = (from i in ls
                                                                             where i.SanLuong == 0
                                                                             select new HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop
                                                                             {
                                                                                 SanLuong = i.SanLuong.Value,
                                                                                 IDLoaiApGia = i.IDLoaiApGia.Value
                                                                             }).ToList();
            sorted.AddRange(sanLuongZero);
            return sorted;
        }

        /// <summary>
        /// Dành cho khách hàng áp giá tổng hợp, tách chỉ số giá tổng hợp ra.
        /// </summary>
        public void tachSoTongHop(int HoaDonID, int cachTinh, int KhachHangID, double SanLuong)
        {
            List<Apgiatonghop> _ls = db.Apgiatonghops.Where(p => p.KhachhangID == KhachHangID).ToList();

            double sanLuongThuc = 0;

            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            if (chiTiet != null)
            {
                chiTiet.SH1 = 0;
                chiTiet.SH2 = 0;
                chiTiet.SH3 = 0;
                chiTiet.SH4 = 0;
                chiTiet.CC = 0;
                chiTiet.HC = 0;
                chiTiet.SXXD = 0;
                chiTiet.KDDV = 0;
                db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //tính bằng giá khoán nếu cách tính = 0 (false) hoặc cách tính = -1 (giá đặc biệt)
                if (cachTinh == 0 || cachTinh == -1)
                {
                    List<HoaDonNuocHaDong.Models.ApGia.ApGiaTongHop> ls = sortApGiaTongHopBasedOnPriority(_ls);
                    foreach (var item in ls)
                    {
                        if (item.SanLuong > 0)
                        {
                            if (item.SanLuong >= SanLuong)
                            {
                                sanLuongThuc = SanLuong;
                            }
                            else
                            {
                                sanLuongThuc = item.SanLuong.Value;
                            }

                            if (item.IDLoaiApGia == KhachHang.SINHHOAT)
                            {
                                int dinhMucCoSo = 10;
                                List<double> chiaChiSoSinhHoatTuongUng = chiaChiSoSinhHoat(sanLuongThuc, dinhMucCoSo, KhachHangID);
                                chiTiet.SH1 = chiaChiSoSinhHoatTuongUng[0];
                                chiTiet.SH2 = chiaChiSoSinhHoatTuongUng[1];
                                chiTiet.SH3 = chiaChiSoSinhHoatTuongUng[2];
                                chiTiet.SH4 = chiaChiSoSinhHoatTuongUng[3];
                            }

                            else if (item.IDLoaiApGia == KhachHang.KINHDOANHDICHVU)
                            {
                                chiTiet.KDDV = sanLuongThuc;
                            }

                            else if (item.IDLoaiApGia == KhachHang.SANXUAT)
                            {
                                chiTiet.SXXD = sanLuongThuc;
                            }

                            else if (item.IDLoaiApGia == KhachHang.COQUANHANHCHINH)
                            {
                                chiTiet.HC = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.DONVISUNGHIEP)
                            {
                                chiTiet.CC = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH1)
                            {
                                chiTiet.SH1 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH2)
                            {
                                chiTiet.SH2 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH3)
                            {
                                chiTiet.SH3 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH4)
                            {
                                chiTiet.SH4 = sanLuongThuc;
                            }
                            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            if (SanLuong - sanLuongThuc >= 0)
                            {
                                SanLuong = SanLuong - sanLuongThuc;
                            }
                        }

                        else
                        {
                            sanLuongThuc = SanLuong;

                            if (item.IDLoaiApGia == KhachHang.SINHHOAT)
                            {
                                int dinhMucCoSo = 10;
                                List<double> chiaChiSoSinhHoatTuongUng = chiaChiSoSinhHoat(sanLuongThuc, dinhMucCoSo, KhachHangID);
                                chiTiet.SH1 = chiaChiSoSinhHoatTuongUng[0];
                                chiTiet.SH2 = chiaChiSoSinhHoatTuongUng[1];
                                chiTiet.SH3 = chiaChiSoSinhHoatTuongUng[2];
                                chiTiet.SH4 = chiaChiSoSinhHoatTuongUng[3];
                            }
                            else if (item.IDLoaiApGia == KhachHang.SANXUAT)
                            {
                                chiTiet.SXXD = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.KINHDOANHDICHVU)
                            {
                                chiTiet.KDDV = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.COQUANHANHCHINH)
                            {
                                chiTiet.HC = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.DONVISUNGHIEP)
                            {
                                chiTiet.CC = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH1)
                            {
                                chiTiet.SH1 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH2)
                            {
                                chiTiet.SH2 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH3)
                            {
                                chiTiet.SH3 = sanLuongThuc;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH4)
                            {
                                chiTiet.SH4 = sanLuongThuc;
                            }

                            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                    }
                }

                //tính bằng giá phần trăm, bổ giá phần trăm                  
                else
                {
                    double SanLuongTuongUng = 0;
                    double SanLuongTong = 0;
                    int recordLeftAfterIteration = _ls.Count();
                    foreach (var item in _ls)
                    {
                        if (item.SanLuong != 0)
                        {
                            if (item.IDLoaiApGia == KhachHang.KINHDOANHDICHVU)
                            {
                                double _SanLuongTuongUng = KhachHang.roundNumber(item.SanLuong.Value / 100 * SanLuong);
                                chiTiet.KDDV = _SanLuongTuongUng;
                                SanLuongTuongUng = _SanLuongTuongUng;
                                SanLuongTong += _SanLuongTuongUng;
                            }

                            else if (item.IDLoaiApGia == KhachHang.SANXUAT)
                            {
                                if (recordLeftAfterIteration == 1)
                                {
                                    double _SanLuongTuongUng = SanLuong - SanLuongTuongUng;
                                    chiTiet.SXXD = _SanLuongTuongUng;
                                }
                                else
                                {
                                    double _SanLuongTuongUng = KhachHang.roundNumber(item.SanLuong.Value / 100 * SanLuong);
                                    chiTiet.SXXD = _SanLuongTuongUng;
                                    SanLuongTuongUng = _SanLuongTuongUng;
                                    SanLuongTong += _SanLuongTuongUng;
                                }
                            }
                            else if (item.IDLoaiApGia == KhachHang.COQUANHANHCHINH)
                            {
                                if (recordLeftAfterIteration == 1)
                                {
                                    double _SanLuongTuongUng = SanLuong - SanLuongTuongUng;
                                    chiTiet.HC = _SanLuongTuongUng;
                                }
                                else
                                {
                                    double _SanLuongTuongUng = KhachHang.roundNumber(item.SanLuong.Value / 100 * SanLuong);
                                    chiTiet.HC = _SanLuongTuongUng;
                                    SanLuongTuongUng = _SanLuongTuongUng;
                                    SanLuongTong += _SanLuongTuongUng;
                                }
                            }
                            else if (item.IDLoaiApGia == KhachHang.DONVISUNGHIEP)
                            {
                                if (recordLeftAfterIteration == 1)
                                {
                                    double _SanLuongTuongUng = SanLuong - SanLuongTuongUng;
                                    chiTiet.CC = _SanLuongTuongUng;
                                }
                                else
                                {
                                    double _SanLuongTuongUng = KhachHang.roundNumber(item.SanLuong.Value / 100 * SanLuong);
                                    chiTiet.CC = _SanLuongTuongUng;
                                    SanLuongTuongUng = _SanLuongTuongUng;
                                    SanLuongTong += _SanLuongTuongUng;
                                }
                            }
                            else if (item.IDLoaiApGia == KhachHang.SINHHOAT)
                            {
                                double SanLuongConLai = SanLuong - SanLuongTong;
                                double _TongSoTieuThu = SanLuongConLai;
                                int dinhMucCoSo = 10;
                                List<double> chiaChiSoSinhHoatTuongUng = chiaChiSoSinhHoat(_TongSoTieuThu, dinhMucCoSo, KhachHangID);
                                chiTiet.SH1 = chiaChiSoSinhHoatTuongUng[0];
                                chiTiet.SH2 = chiaChiSoSinhHoatTuongUng[1];
                                chiTiet.SH3 = chiaChiSoSinhHoatTuongUng[2];
                                chiTiet.SH4 = chiaChiSoSinhHoatTuongUng[3];
                            }
                            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        recordLeftAfterIteration--;
                    }
                }//end cach tinh = 1 
            }
        }

        private List<double> chiaChiSoSinhHoat(double _TongSoTieuThu, int dinhMucCoSo, int KhachHangID)
        {
            List<double> luuTruChiSo = new List<double>();
            int soNhanKhau = kHHelper.getSoNhanKhau(KhachHangID);
            int soHo = kHHelper.getSoHo(KhachHangID);

            double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
            double SH1, SH2, SH3, SH4;
            SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;

            double dinhMucSH1 = dinhMucTungNha;
            double dinhMucSH2 = dinhMucTungNha * 2;
            double dinhMucSH3 = dinhMucTungNha * 3;


            if (_TongSoTieuThu - SH1 <= 0)
            {
                SH2 = 0;
            }
            else
            {
                SH2 = _TongSoTieuThu - SH1 <= (dinhMucSH2 - dinhMucSH1) ? _TongSoTieuThu - SH1 : (dinhMucSH2 - dinhMucSH1);
            }

            if (_TongSoTieuThu - SH1 - SH2 <= 0)
            {
                SH3 = 0;
            }
            else
            {
                SH3 = _TongSoTieuThu - SH1 - SH2 <= (dinhMucSH3 - dinhMucSH2) ? _TongSoTieuThu - SH1 - SH2 : (dinhMucSH3 - dinhMucSH2);
            }

            if (_TongSoTieuThu - SH1 - SH2 - SH3 <= 0)
            {
                SH4 = 0;
            }
            else
            {
                SH4 = _TongSoTieuThu - SH1 - SH2 - SH3;
            }

            luuTruChiSo.Add(SH1);
            luuTruChiSo.Add(SH2);
            luuTruChiSo.Add(SH3);
            luuTruChiSo.Add(SH4);

            return luuTruChiSo;
        }

        [HttpPost]
        public ActionResult chotSoLieu(int tuyenID, int month, int year)
        {
            cS.capNhatTrangThaiChotTuyen(tuyenID, month, year);
            cS.capnhatTrangThaiDanhSachHoaDonBiHuyThuocTuyen(tuyenID, month, year);
            cS.capNhatTrangThaiChotHoaDon(tuyenID, month, year);
            saochepDanhsachKhachHangKhongSanLuong(tuyenID, month, year);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Hàm dùng để sao chép danh sách khách hàng không sản lượng từ tháng trước
        /// </summary>
        ///<param name="currentMonth"></param>
        ///<param name="currentYear"></param>
        public void saochepDanhsachKhachHangKhongSanLuong(int tuyenID, int currentMonth, int currentYear)
        {
            int nextMonth = currentMonth + 1 > 12 ? 1 : currentMonth + 1;
            int nextYear = currentMonth + 1 > 12 ? currentYear + 1 : currentYear;

            SqlConnection conn = new SqlConnection(HoaDonNuocHaDong.Config.DatabaseConfig.getConnectionString());
            conn.Open();

            ControllerBase<DanhSachHoaDonKhongSanLuong> cB = new ControllerBase<DanhSachHoaDonKhongSanLuong>();
            List<DanhSachHoaDonKhongSanLuong> dsKhachHangKoSanLuong = cB.Query("DanhSachHoaDonKhongSanLuong", new SqlParameter("@thang", currentMonth),
                new SqlParameter("@nam", currentYear), new SqlParameter("@tuyen", tuyenID)).ToList();

            if (dsKhachHangKoSanLuong.Count > 0)
            {
                foreach (var dsHoaDonThangSau in dsKhachHangKoSanLuong)
                {
                    Hoadonnuoc khongSanLuong = new Hoadonnuoc();
                    khongSanLuong.ThangHoaDon = nextMonth;
                    khongSanLuong.NamHoaDon = nextYear;
                    khongSanLuong.Ngaybatdausudung = dsHoaDonThangSau.NgayBatDauSuDung;
                    khongSanLuong.KhachhangID = dsHoaDonThangSau.KhachHangID;
                    khongSanLuong.NhanvienID = LoggedInUser.NhanvienID;
                    khongSanLuong.Trangthaixoa = false;
                    khongSanLuong.Trangthaithu = false;
                    khongSanLuong.Trangthaiin = false;
                    khongSanLuong.Trangthaichot = false;
                    db.Hoadonnuocs.Add(khongSanLuong);
                    db.SaveChanges();
                    //Thêm chi tiết hóa đơn nước tháng sau
                    Chitiethoadonnuoc chiTiet = new Chitiethoadonnuoc();
                    chiTiet.HoadonnuocID = khongSanLuong.HoadonnuocID;
                    chiTiet.Chisocu = dsHoaDonThangSau.ChiSoCu;
                    db.Chitiethoadonnuocs.Add(chiTiet);
                    db.SaveChanges();
                }
            }

            conn.Close();
        }
        /// <summary>
        /// load hóa đơn có chỉ số không bình thường lên, giống vs hàm load danh sách hóa đơn, 
        /// chỉ khác là load danh sách hóa đơn có sản lượng nho hon 1 và gấp đôi tháng trước
        /// </summary>
        /// <returns></returns>

        public ActionResult LoadKhongBt(int tuyenID, int month, int year, int nhanvienInt)
        {
            ViewBag.year = year;
            ViewBag.month = month;
            DateTime thangNamGanNhat = cS.getThangNamGanNhatThuocHoaDon(tuyenID, month, year);
            int previousMonth = thangNamGanNhat.Month;
            int previousYear = thangNamGanNhat.Year;

            ViewBag.khachHangBatThuong = cS.loadDanhSachSanLuongBatThuong(previousMonth, previousYear, month, year, tuyenID).Distinct().ToList();
            ViewBag.currentDate = String.Concat(DateTime.Now.Day, '/', DateTime.Now.Month);
            ViewBag.nextMonth = String.Concat(DateTime.Now.Day, '/', DateTime.Now.Month + 1 > 12 ? 1 : DateTime.Now.Month + 1);
            ViewData["nhanVienObj"] = db.Nhanviens.Find(nhanvienInt);
            ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenID);
            return View();
        }


        public int getPreviousMonthYear(int month, int year, bool getMonth)
        {
            int previousMonth = month - 1 == 0 ? 12 : month - 1;
            int previousYear = month - 1 == 0 ? year - 1 : year;
            if (getMonth == true) return previousMonth;
            return previousYear;
        }

        private int getSanLuongThangTruocCuaKhachHang(int month, int year, int khachhangId)
        {
            var sanLuongThangTruoc = (from i in db.Hoadonnuocs
                                      where i.ThangHoaDon == month && i.NamHoaDon == year && i.KhachhangID == khachhangId
                                      select new Models.SoLieuTieuThu.HoaDonNuoc
                                      {
                                          SanLuong = i.Tongsotieuthu,
                                      }).FirstOrDefault();
            if (sanLuongThangTruoc == null)
            {
                return -1;
            }
            return sanLuongThangTruoc.SanLuong == null ? 0 : sanLuongThangTruoc.SanLuong.Value;
        }

        /// <summary>
        /// Cập nhật ngày bắt đầu của hóa đơn (thời gian sử dụng bắt đầu trong tháng)
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="ngayBatDau"></param>
        public void capnhatbatdau(int HoaDonID, string ngayBatDau)
        {
            Lichsuhoadon ls = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == HoaDonID);
            ls.NgayBatDau = ngayBatDau.ToString();
            db.Entry(ls).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            //cập nhật ngày bắt đầu trong bảng hóa đơn
            Hoadonnuoc hoaDon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            if (hoaDon != null)
            {
                hoaDon.Ngaybatdausudung = Convert.ToDateTime(ngayBatDau);
                db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }




        /// <summary>
        /// Cập nhật ngày kết thúc hóa đơn (thời gian kết thúc sử dụng trong tháng)
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="ngayKetThuc"></param>
        public void capnhatketthuc(String HoaDonID, String KhachHangID, String ngayKetThuc, String thangNay, String thangSau, String namNay, String namSau)
        {
            int hdID = Convert.ToInt32(HoaDonID);
            Lichsuhoadon ls = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hdID);
            if (ls != null)
            {
                ls.NgayKetThuc = ngayKetThuc;
                db.Entry(ls).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            //cập nhật ngày bắt đầu sử dụng của tháng sau luôn
            int khID = Convert.ToInt32(KhachHangID);
            int thangNayToInt = Convert.ToInt16(thangNay);
            int namNayToInt = Convert.ToInt16(namNay);
            int thangSauToInt = Convert.ToInt16(thangSau);
            int namSauToInt = Convert.ToInt16(namSau);

            Hoadonnuoc hD = db.Hoadonnuocs.FirstOrDefault(p => p.ThangHoaDon == thangNayToInt && p.NamHoaDon == namNayToInt && p.KhachhangID == khID);
            if (hD != null)
            {
                hD.Ngayketthucsudung = Convert.ToDateTime(ngayKetThuc);
                db.Entry(hD).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            //cập nhật tháng sau nếu có
            Hoadonnuoc hDThangSau = db.Hoadonnuocs.FirstOrDefault(p => p.ThangHoaDon == thangSauToInt && p.NamHoaDon == namSauToInt && p.KhachhangID == khID);
            if (hDThangSau != null)
            {
                hDThangSau.Ngaybatdausudung = Convert.ToDateTime(ngayKetThuc);
                db.Entry(hDThangSau).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public JsonResult loadApGiaInfo(int KhachHangID, int thang, int nam)
        {
            var apGiaTongHop = (from i in db.Apgiatonghops
                                where i.KhachhangID == KhachHangID
                                select new
                                {
                                    ID = i.Id,
                                    CachTinh = i.CachTinh,
                                    SanLuong = i.SanLuong,
                                    LoaiApGia = i.IDLoaiApGia
                                }).ToList();
            return Json(apGiaTongHop, JsonRequestBehavior.AllowGet);
        }

    }//end class
}//end namespace