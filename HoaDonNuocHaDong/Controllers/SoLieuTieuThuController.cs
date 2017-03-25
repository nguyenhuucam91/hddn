using HDNHD.Models.Constants;
using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class SoLieuTieuThuController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        private ChiSo cS = new ChiSo();
        private HoaDonNuoc hD = new HoaDonNuoc();
        private KhachHang kHHelper = new KhachHang();
        private NguoidungHelper ngDungHelper = new NguoidungHelper();


        // GET: /SoLieuTieuThu/
        /// <summary>
        /// Hiển thị danh sách chi nhánh, tổ, nhân viên, tuyến và khách hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? to, int? nhanvien, int? tuyen, int? thang, int? nam)
        {
            int quanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            ViewBag.selectedChiNhanh = quanHuyenID;
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);

            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

            List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.to = toLs;
            ViewBag.showHoaDon = false;
            //load danh sách nhân viên thuộc tổ có phòng ban đó.
            if (nhanvien == null)
            {
                List<Nhanvien> nVLs = new List<Nhanvien>();
                ViewBag.nhanVien = nVLs;
            }
            else
            {
                List<Nhanvien> _nvLs = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == to && (p.IsDelete == false || p.IsDelete == null)).ToList();
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
                List<Tuyenkhachhang> tuyensLs = new List<Tuyenkhachhang>();

                var tuyenTheoNhanVien = (from i in db.Tuyentheonhanviens
                                         join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                         join s in db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                         join q in db.Phongbans on s.PhongbanID equals q.PhongbanID
                                         where i.NhanVienID == nhanvien
                                         select r).ToList();
                tuyensLs.AddRange(tuyenTheoNhanVien);
                ViewBag.tuyen = tuyensLs;
                //lấy danh sách khách hàng thuộc tuyến đó
                List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cS.filterChiSo(thang.Value, nam.Value, tuyen.Value);
                ViewBag.khachHang = chiSoTieuThu;
                ViewBag.showHoaDon = true;
                ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyen);
                Nhanvien nhanVienObj = db.Nhanviens.Find(nhanvien);
                ViewData["nhanVienObj"] = nhanVienObj;
            }

            //load viewBag ngày bắt đầu            
            ViewBag.month = thang == null ? DateTime.Now.Month : thang;
            ViewBag.year = nam == null ? DateTime.Now.Year : nam;
            //kiểm đinh            
            ViewBag.ngayBatDau = "";
            ViewBag.ngayKetThuc = "";
            //danh sách lỗi
            List<string> errors = new List<string>();
            ViewData["errorList"] = errors;
            //view
            ViewBag.selectedTo = to;
            ViewBag.selectedTuyen = tuyen;
            ViewBag.selectedNhanVien = nhanvien;
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
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
            int quanHuyenID = selectedQuanHuyenID;
            //generate chi tiet hóa đơn nước tháng sau
            int nhanVienInt = String.IsNullOrEmpty(form["nhanvien"]) ? 0 : Convert.ToInt32(form["nhanvien"]);
            //gán session cho nhân viên để tiến hành thêm mới hóa đơn tháng sau cũng như lấy nhân viên cho các controller khác có thể tác động vào.          
            Session["nhanvien"] = nhanVienInt;
            //năm và tháng được chọn
            String month = form["thang"];
            String year = form["nam"];
            //nếu năm tháng rỗng thì lấy năm và tháng hiện tại, nếu tuyến được chọn rỗng thì lấy là 0
            int _month = String.IsNullOrEmpty(month) ? DateTime.Now.Month : Convert.ToInt16(month);
            int _year = String.IsNullOrEmpty(year) ? DateTime.Now.Year : Convert.ToInt16(year);

            String selectedNhanVien = form["nhanvien"];
            String selectedTuyen = form["tuyen"];
            //lấy danh sách tổ, phòng ban thuộc tổ quận huyện đó
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
            int phongBanID = phongBan.PhongbanID;

            //khởi tạo danh sách lỗi
            List<String> errorList = new List<String>();
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
                List<Nhanvien> _nvLs = db.Nhanviens.Where(p => p.ToQuanHuyenID == toForm && (p.IsDelete == false || p.IsDelete == null)).ToList();
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
                    Nhanvien nhanVienObj = db.Nhanviens.Find(nhanVienInt);
                    ViewData["nhanVienObj"] = nhanVienObj;
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
                    Session["solieuTieuThuNhanvien"] = Convert.ToInt32(selectedNhanVien);
                    //nếu tuyến để trống thì thêm mới lỗi vào errorList
                    if (String.IsNullOrEmpty(selectedTuyen))
                    {
                        errorList.Add("Không nhập được số liệu tiêu thụ do tuyến để trống");
                        ViewBag.tuyen = new List<Tuyenkhachhang>();
                    }
                    else
                    {
                        int tuyenInt = Convert.ToInt32(selectedTuyen);
                        //sao chép ds khách hàng không sản lượng vào tháng hiện tại                       
                        ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenInt);
                        List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cS.filterChiSo(_month, _year, tuyenInt);
                        ViewBag.khachHang = chiSoTieuThu;
                        ViewBag.selectedNhanvien = Session["solieuTieuThuNhanvien"];
                        ViewBag.selectedTuyen = tuyenInt;
                        ViewBag.selectedTo = toForm;
                    }
                }
            }



            int _selectedTuyen = String.IsNullOrEmpty(selectedTuyen) ? 0 : Convert.ToInt32(selectedTuyen);
            //lấy danh sách quận huyện để đẩy vào phần lọc chỉ số KH
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 1);
            //dành cho người dùng
            ViewBag.showHoaDon = true;
            ViewBag.month = _month;
            ViewBag.year = _year;

            ViewBag.nextMonth = "";

            ViewData["errorList"] = errorList;

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
        public void NhapChiSoMoi(int HoaDonID, int? ChiSoDau, int? ChiSoCuoi, int? TongSoTieuThu, int SoKhoan, int KHID, int SoHoaDon, String dateStart, String dateEnd, String dateInput, int thang, int nam)
        {
            int _month = thang;
            int _year = nam;
            int _TongSoTieuThu = 0;
            int _tongKiemDinh = 0;

            Hoadonnuoc hoaDon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            //nếu có record hóa đơn trọng hệ thống
            if (hoaDon != null)
            {
                //nếu session người dùng ID khác null thì lấy người dùng đăng nhập hệ thống, nếu không lấy NhanVienID = 0;
                if (Session["nhanVienID"] != null)
                {
                    hoaDon.NhanvienID = int.Parse(Session["nhanVienID"].ToString());
                }

                var isKiemDinh = HoaDonNuocHaDong.Helper.KiemDinh.checkKiemDinhStatus(KHID, _month, _year);
                //nếu khách hàng đang chỉnh sửa chưa kiểm định mà nhập số thì tính như bình thường
                if (isKiemDinh)
                {
                    var kiemDinh1 = HoaDonNuocHaDong.Helper.KiemDinh.getChiSoLucKiemDinh(KHID, _month, _year) - ChiSoDau.Value;
                    var kiemDinh2 = ChiSoCuoi.Value - HoaDonNuocHaDong.Helper.KiemDinh.getChiSoSauKiemDinh(KHID, _month, _year);
                    _tongKiemDinh = kiemDinh1 + kiemDinh2;
                    _TongSoTieuThu = _tongKiemDinh;
                }
                else
                {
                    _TongSoTieuThu = TongSoTieuThu.Value;
                }
                hoaDon.Ngayhoadon = Convert.ToDateTime(dateInput);
                hoaDon.Ngaybatdausudung = Convert.ToDateTime(dateStart);
                hoaDon.Ngayketthucsudung = Convert.ToDateTime(dateEnd);
                hoaDon.Tongsotieuthu = _TongSoTieuThu;
                db.SaveChanges();
            }

            //tách chỉ số
            tachChiSoSanLuong(HoaDonID, ChiSoDau.Value, ChiSoCuoi.Value, _TongSoTieuThu, SoKhoan, KHID);

            //thêm 1 records số tiền phải nộp vào tháng sau với ngày kết thúc của tháng này là ngày bắt đầu của tháng sau
            HoaDonNuoc.themMoiHoaDonThangSau(KHID, HoaDonID, ChiSoCuoi.Value, Convert.ToInt32(Session["nhanvien"]), _month, _year, Convert.ToDateTime(dateEnd));
            hD.themMoiSoTienPhaiNop(HoaDonID);
            //lấy 2 object đẩy vào 

            //thêm mới record vào bảng lịch sử sử dụng nước để in

            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);
            Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);

            //tongTienHoaDon;
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
            double thueBVMT = cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value);
            double tongTienHoaDon = dinhMuc + thueBVMT + VAT;
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
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc).Sum(p => p.TongCong);
            }
            double tongCongCongDon = Convert.ToDouble(tongTienHoaDon + congDonHDTruoc);

            insertToLichSuSuDungNuoc(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, _TongSoTieuThu,
                cT.SH1.Value, cS.getSoTienTheoApGia("SH1").Value,
                cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value,
                cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value,
                cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
                cT.HC.Value, cS.getSoTienTheoApGia("HC").Value,
                cT.CC.Value, cS.getSoTienTheoApGia("CC").Value,
                cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value,
                cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
                5, VAT,
                obj.Tilephimoitruong.Value, thueBVMT, tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
                thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, dateStart, dateEnd);
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
                    int dinhMucCoSo = apGia[0].Denmuc.Value;
                    double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
                    ////40 => 16                    
                    double SH1, SH2, SH3, SH4;
                    SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;
                    ////40-16 = 24<=20 ? 4:20
                    ////33-16 = 17<=20 ? 17:20
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
                    //nếu khách hàng là kinh doanh 
                    if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.KINHDOANHDICHVU)
                    {
                        chiTiet.SH1 = 0;
                        chiTiet.SH2 = 0;
                        chiTiet.SH3 = 0;
                        chiTiet.SH4 = 0;
                        chiTiet.CC = 0;
                        chiTiet.HC = 0;
                        chiTiet.SXXD = 0;
                        chiTiet.KDDV = _TongSoTieuThu;
                    }
                    //Sản xuất
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.SANXUAT)
                    {
                        chiTiet.SH1 = 0;
                        chiTiet.SH2 = 0;
                        chiTiet.SH3 = 0;
                        chiTiet.SH4 = 0;
                        chiTiet.CC = 0;
                        chiTiet.HC = 0;
                        chiTiet.KDDV = 0;
                        chiTiet.SXXD = _TongSoTieuThu;
                    }
                    //đơn vị sự nghiệp
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.DONVISUNGHIEP)
                    {
                        chiTiet.SH1 = 0;
                        chiTiet.SH2 = 0;
                        chiTiet.SH3 = 0;
                        chiTiet.SH4 = 0;
                        chiTiet.HC = 0;
                        chiTiet.KDDV = 0;
                        chiTiet.SXXD = 0;
                        chiTiet.CC = _TongSoTieuThu;
                    }
                    //kinh doanh dịch vụ
                    else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.COQUANHANHCHINH)
                    {
                        chiTiet.SH1 = 0;
                        chiTiet.SH2 = 0;
                        chiTiet.SH3 = 0;
                        chiTiet.SH4 = 0;
                        chiTiet.CC = 0;
                        chiTiet.KDDV = 0;
                        chiTiet.SXXD = 0;
                        chiTiet.HC = _TongSoTieuThu;
                    }
                }
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
            HoaDonNuoc.themMoiHoaDonThangSau(KHID, HoaDonID, ChiSoCuoi.Value, Convert.ToInt32(Session["nhanvien"]), _month, _year, Convert.ToDateTime(dateEnd));
            hD.themMoiSoTienPhaiNop(HoaDonID);
            //thêm vào bảng lịch sử sử dụng nước
            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);

            Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            //tongTienHoaDon;
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
            double thueBVMT = cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value);

            double tongTienHoaDon = dinhMuc + thueBVMT + VAT;
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
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc).Sum(p => p.TongCong);
            }
            double tongCongCongDon = tongTienHoaDon + congDonHDTruoc;

            insertToLichSuSuDungNuoc(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, TongSoTieuThu.Value, cT.SH1.Value,
                cS.getSoTienTheoApGia("SH1").Value, cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value, cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value, cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
                cT.HC.Value, cS.getSoTienTheoApGia("HC").Value, cT.CC.Value, cS.getSoTienTheoApGia("CC").Value, cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value, cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
                5, VAT,
                obj.Tilephimoitruong.Value, thueBVMT,
                tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3, thuNgan, obj.TuyenKHID.Value,
                obj.TTDoc.Value, tongCongCongDon, dateStart, dateEnd);
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

            var chiSoTieuThu = (from i in db.Hoadonnuocs
                                join r in db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                join m in db.Chitiethoadonnuocs on i.HoadonnuocID equals m.HoadonnuocID
                                join l in db.Loaiapgias on r.LoaiapgiaID equals l.LoaiapgiaID
                                where i.ThangHoaDon == _month && i.NamHoaDon == _year &&
                                r.TuyenKHID == tuyenID && (i.Trangthaixoa == false || i.Trangthaixoa == null)
                                select new HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc
                                {
                                    HoaDonNuocID = i.HoadonnuocID,
                                    MaKhachHang = r.MaKhachHang,
                                    ThuTuDoc = r.TTDoc,
                                    TenKhachHang = r.Ten,
                                    LoaiApGia = l.Ten,
                                    LoaiApGiaID = r.LoaiapgiaID,
                                    SoHo = r.Soho,
                                    SoKhau = r.Sonhankhau,
                                    ChiSoCu = m.Chisocu,
                                    ChiSoMoi = m.Chisomoi,
                                    SoKhoan = i.SoKhoan,
                                    SanLuong = i.Tongsotieuthu,
                                    SH1 = m.SH1 == 0 ? "" : m.SH1.ToString(),
                                    SH2 = m.SH2 == 0 ? "" : m.SH2.ToString(),
                                    SH3 = m.SH3 == 0 ? "" : m.SH3.ToString(),
                                    SH4 = m.SH4 == 0 ? "" : m.SH4.ToString(),
                                    HC = m.HC == 0 ? "" : m.HC.ToString(),
                                    CC = m.CC == 0 ? "" : m.CC.ToString(),
                                    SXXD = m.SXXD == 0 ? "" : m.SXXD.ToString(),
                                    KDDV = m.KDDV == 0 ? "" : m.KDDV.ToString(),
                                    Thang = _month,
                                    Nam = _year,
                                    KHID = r.KhachhangID,
                                }).OrderBy(p => p.ThuTuDoc).ToList();
            ViewBag.chiSoTieuThu = chiSoTieuThu;
            ViewBag.trangthaiChotTuyen = db.TuyenDuocChots.Count(p => p.TuyenKHID == tuyenID && p.Thang == _month && p.Nam == _year);
            ViewBag.soLuongHoaDonCoSanLuong = chiSoTieuThu.Count(p => p.SanLuong > 1);
            ViewBag.soLuongHoaDonKhongCoSanLuong = chiSoTieuThu.Count(p => p.SanLuong <= 1);
            ViewBag.soLuongHoaDon = chiSoTieuThu.Count();
            ViewData["nhanvien"] = db.Nhanviens.Find(nvquanly);
            ViewData["tuyen"] = db.Tuyenkhachhangs.Find(tuyenID);
            return View();
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
                           where i.KhachhangID == KhachHangID && i.Ngaykiemdinh.Value.Month == month && i.Ngaykiemdinh.Value.Year == year
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
                ViewBag.sanLuong = hoaDonNuoc.HoaDonNuoc.Tongsotieuthu;
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
        public ActionResult Nhapgiadacbiet(int? id, FormCollection form, int to, int nhanvien, int tuyen, int thang, int nam)
        {
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
            //reset chỉ số của tháng đó trước, cho về 0, nếu có ấn nhập giá đặc biệt

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

            hD.themMoiSoTienPhaiNop(id.Value);
            //thêm vào bảng lịch sử
            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);

            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);
            int _month = hoaDonDacBiet.ThangHoaDon.Value;
            int _year = hoaDonDacBiet.NamHoaDon.Value;
            int ChiSoDau = chiTiet.Chisocu.Value;
            int ChiSoCuoi = chiTiet.Chisomoi.Value;
            //Giá 
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(id.Value, chiTiet.SH1.Value, chiTiet.SH2.Value, chiTiet.SH3.Value, chiTiet.SH4.Value, chiTiet.HC.Value, chiTiet.CC.Value, chiTiet.KDDV.Value, chiTiet.SXXD.Value);
            double thue = cS.tinhThue(id.Value, chiTiet.SH1.Value, chiTiet.SH2.Value, chiTiet.SH3.Value, chiTiet.SH4.Value, chiTiet.HC.Value, chiTiet.CC.Value, chiTiet.KDDV.Value, chiTiet.SXXD.Value, obj.Tilephimoitruong.Value);
            double VAT = Math.Round(dinhMuc * 0.05, MidpointRounding.AwayFromZero);
            //chỉ số cộng dồn
            double tongTienHoaDon = dinhMuc + thue + VAT;
            double congDonHDTruoc = 0;
            //kiểm tra, nếu là record đầu tiên
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc);
            if (count == 0)
            {
                congDonHDTruoc = 0;
            }
            else
            {
                congDonHDTruoc = db.Lichsuhoadons.Where(p => p.TuyenKHID == obj.TuyenKHID.Value && p.ThangHoaDon == _month && p.NamHoaDon == _year && p.TTDoc < obj.TTDoc).Sum(p => p.TongCong);
            }
            double tongCongCongDon = tongTienHoaDon + congDonHDTruoc;
            //thu ngân
            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon;

            insertToLichSuSuDungNuoc(id.Value, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau, ChiSoCuoi, Sum,
                  SH1, cS.getSoTienTheoApGia("SH1").Value,
                  SH2, cS.getSoTienTheoApGia("SH2").Value,
                  SH3, cS.getSoTienTheoApGia("SH3").Value,
                  SH4, cS.getSoTienTheoApGia("SH4").Value,
                  HC, cS.getSoTienTheoApGia("HC").Value,
                  CC, cS.getSoTienTheoApGia("CC").Value,
                  SX, cS.getSoTienTheoApGia("SX-XD").Value,
                  KD, cS.getSoTienTheoApGia("KDDV").Value,
                  5, VAT, obj.Tilephimoitruong.Value, thue,
                  tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                  db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
                  thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, startDate, endDate);

            return RedirectToAction("Index", new { to = to, nhanvien = nhanvien, tuyen = tuyen, thang = thang, nam = nam });
        }

        public string insertToLichSuSuDungNuoc(int HoaDonID, int thangHoaDon, int namHoaDon, String tenKH, String diaChi, String MST, String maKH, int TuyenKHID, String soHD,
            int chiSoCu, int ChiSoMoi, int TongTieuThu, double SH1, double SH1Price, double SH2, double SH2Price, double SH3, double SH3Price, double SH4, double SH4Price, double HC, double doubleHCPrice,
            double CC, double CCPrice, double SX, double SXPrice, double KD, double KDPrice, double Thue, double TienThueVAT, double TileBVMT,
            double BVMTPrice, double TongCong, String bangChu, String TTVoOng, String ThuNgan, int tuyen, int TTDoc, double chiSoCongDon, string ngayBatDau, string ngayKetThuc)
        {
            Lichsuhoadon lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == HoaDonID);
            if (lichSuHoaDon != null)
            {
                lichSuHoaDon.HoaDonID = HoaDonID;
                lichSuHoaDon.ThangHoaDon = thangHoaDon;
                lichSuHoaDon.NamHoaDon = namHoaDon;
                lichSuHoaDon.TenKH = tenKH;
                lichSuHoaDon.Diachi = diaChi;
                lichSuHoaDon.MST = MST == null ? "" : MST;
                lichSuHoaDon.MaKH = maKH;
                lichSuHoaDon.TuyenKHID = TuyenKHID;
                lichSuHoaDon.SoHopDong = soHD;
                lichSuHoaDon.ChiSoCu = chiSoCu;
                lichSuHoaDon.ChiSoMoi = ChiSoMoi;
                lichSuHoaDon.SanLuongTieuThu = TongTieuThu;
                lichSuHoaDon.SH1 = SH1;
                lichSuHoaDon.SH1Price = SH1Price;
                lichSuHoaDon.SH2 = SH2;
                lichSuHoaDon.SH2Price = SH2Price;
                lichSuHoaDon.SH3 = SH3;
                lichSuHoaDon.SH3Price = SH3Price;
                lichSuHoaDon.SH4 = SH4;
                lichSuHoaDon.SH4Price = SH4Price;
                lichSuHoaDon.HC = HC;
                lichSuHoaDon.HCPrice = doubleHCPrice;
                lichSuHoaDon.CC = CC;
                lichSuHoaDon.CCPrice = CCPrice;
                lichSuHoaDon.SX = SX;
                lichSuHoaDon.SXPrice = SXPrice;
                lichSuHoaDon.KD = KD;
                lichSuHoaDon.KDPrice = KDPrice;
                lichSuHoaDon.ThueSuat = Thue;
                lichSuHoaDon.ThueSuatPrice = TienThueVAT;
                lichSuHoaDon.TileBVMT = TileBVMT;
                lichSuHoaDon.PhiBVMT = BVMTPrice;
                lichSuHoaDon.TongCong = TongCong;
                lichSuHoaDon.BangChu = ConvertMoney.So_chu(TongCong);
                lichSuHoaDon.TTVoOng = TTVoOng;
                lichSuHoaDon.TTThungan = ThuNgan;
                lichSuHoaDon.TuyenKHID = tuyen;
                lichSuHoaDon.TTDoc = TTDoc;
                lichSuHoaDon.ChiSoCongDon = chiSoCongDon;
                lichSuHoaDon.NgayBatDau = ngayBatDau;
                lichSuHoaDon.NgayKetThuc = ngayKetThuc;
                db.Entry(lichSuHoaDon).State = System.Data.Entity.EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    String err = "";
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            err += validationError.PropertyName + "" + validationError.ErrorMessage;
                        }
                    }
                    return err;
                }
            }
            else
            {
                Lichsuhoadon history = new Lichsuhoadon();
                history.HoaDonID = HoaDonID;
                history.ThangHoaDon = thangHoaDon;
                history.NamHoaDon = namHoaDon;
                history.TenKH = tenKH;
                history.Diachi = diaChi;
                history.MST = MST == null ? "" : MST;
                history.TuyenKHID = TuyenKHID;
                history.MaKH = maKH;
                history.SoHopDong = soHD;
                history.ChiSoCu = chiSoCu;
                history.ChiSoMoi = ChiSoMoi;
                history.SanLuongTieuThu = TongTieuThu;
                history.SH1 = SH1;
                history.SH1Price = SH1Price;
                history.SH2 = SH2;
                history.SH2Price = SH2Price;
                history.SH3 = SH3;
                history.SH3Price = SH3Price;
                history.SH4 = SH4;
                history.SH4Price = SH4Price;
                history.HC = HC;
                history.HCPrice = doubleHCPrice;
                history.CC = CC;
                history.CCPrice = CCPrice;
                history.SX = SX;
                history.SXPrice = SXPrice;
                history.KD = KD;
                history.KDPrice = KDPrice;
                history.ThueSuat = Thue;
                history.ThueSuatPrice = TienThueVAT;
                history.TileBVMT = TileBVMT;
                history.PhiBVMT = BVMTPrice;
                history.TongCong = TongCong;
                history.BangChu = ConvertMoney.So_chu(TongCong);
                history.TTVoOng = TTVoOng;
                history.TTThungan = ThuNgan;
                history.TuyenKHID = tuyen;
                history.TTDoc = TTDoc;
                history.ChiSoCongDon = chiSoCongDon;
                history.NgayBatDau = ngayBatDau;
                history.NgayKetThuc = ngayKetThuc;
                db.Lichsuhoadons.Add(history);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Response.Write(e.ToString());
                    Response.End();
                }
            }

            return "";
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
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;
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
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;
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
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;
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
            var isChot = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenID && p.Thang == month && p.Nam == year);
            if (isChot == null)
            {
                TuyenDuocChot tuyenChot = new TuyenDuocChot();
                tuyenChot.TuyenKHID = tuyenID;
                tuyenChot.Nam = year;
                tuyenChot.Thang = month;
                tuyenChot.TrangThaiChot = true;
                tuyenChot.TrangThaiTinhTien = false;
                tuyenChot.NgayChot = DateTime.Now;
                db.TuyenDuocChots.Add(tuyenChot);
                db.SaveChanges();
            }
            //cập nhật trạng thái chốt cho tất cả hóa đơn của khách hàng thuộc tuyến đó               
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
            var dsKhachHangKoSanLuong = (from i in db.Hoadonnuocs
                                         join s in db.Khachhangs on i.KhachhangID equals s.KhachhangID
                                         join c in db.Chitiethoadonnuocs on i.HoadonnuocID equals c.HoadonnuocID
                                         where i.ThangHoaDon == currentMonth && i.NamHoaDon == currentYear && s.TuyenKHID == tuyenID && i.Tongsotieuthu == null
                                         select new
                                         {
                                             HoaDonID = i.HoadonnuocID,
                                             ThangHoaDon = i.ThangHoaDon,
                                             Ngaybatdausudung = i.Ngaybatdausudung,
                                             KhachHangID = i.KhachhangID,
                                             ChiSoCu = c.Chisocu
                                         }).ToList();

            foreach (var item in dsKhachHangKoSanLuong)
            {
                Hoadonnuoc nextMonthReceipt = db.Hoadonnuocs.FirstOrDefault(i => i.ThangHoaDon == nextMonth && i.NamHoaDon == nextYear
                    && i.KhachhangID == item.KhachHangID && i.Tongsotieuthu == null);
                if (nextMonthReceipt == null)
                {
                    Hoadonnuoc khongSanLuong = new Hoadonnuoc();
                    khongSanLuong.ThangHoaDon = nextMonth;
                    khongSanLuong.NamHoaDon = nextYear;
                    khongSanLuong.Ngaybatdausudung = item.Ngaybatdausudung;
                    khongSanLuong.KhachhangID = item.KhachHangID;
                    khongSanLuong.NhanvienID = Convert.ToInt32(Session["nhanvien"]);
                    db.Hoadonnuocs.Add(khongSanLuong);
                    //Thêm chi tiết hóa đơn nước tháng sau
                    Chitiethoadonnuoc chiTiet = new Chitiethoadonnuoc();
                    chiTiet.HoadonnuocID = khongSanLuong.HoadonnuocID;
                    chiTiet.Chisocu = item.ChiSoCu;
                    db.Chitiethoadonnuocs.Add(chiTiet);
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// load hóa đơn có chỉ số không bình thường lên, giống vs hàm load danh sách hóa đơn, 
        /// chỉ khác là load danh sách hóa đơn có sản lượng <code><=1</code> và gấp đôi tháng trước
        /// </summary>
        /// <returns></returns>

        public ActionResult LoadKhongBt(int tuyenID, int month, int year, int nhanvienInt)
        {
            ViewBag.year = year;
            ViewBag.month = month;

            int previousMonth = getPreviousMonthYear(month, year, true);
            int previousYear = getPreviousMonthYear(month, year, false);

            var danhSachHoaDonBatThuong = new List<Models.SoLieuTieuThu.HoaDonNuoc>();
            var danhSachHoaDon = (from i in db.Hoadonnuocs
                                  join r in db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                  join m in db.Chitiethoadonnuocs on i.HoadonnuocID equals m.HoadonnuocID
                                  join t in db.Lichsuhoadons on i.HoadonnuocID equals t.HoaDonID
                                  where i.ThangHoaDon == month && i.NamHoaDon == year && r.TuyenKHID == tuyenID
                                  && (r.Tinhtrang == 0 || r.Tinhtrang == null) && (r.IsDelete == false)
                                  orderby r.TTDoc
                                  select new Models.SoLieuTieuThu.HoaDonNuoc
                                  {
                                      HoaDonNuocID = i.HoadonnuocID,
                                      KhachHangID = r.KhachhangID,
                                      MaKhachHang = r.MaKhachHang,
                                      TenKhachHang = r.Ten,
                                      SoHo = r.Soho,
                                      SoKhau = r.Sonhankhau,
                                      ChiSoCu = m.Chisocu,
                                      ChiSoMoi = m.Chisomoi,
                                      SanLuong = i.Tongsotieuthu,
                                      ThuTuDoc = r.TTDoc,
                                      SoHoaDon = t.TTThungan,
                                      SoKhoan = i.SoKhoan
                                  }).ToList();

            //load danh sách khách hàng có áp giá đặc biệt
            foreach (var item in danhSachHoaDon)
            {
                int sanLuongThangTruocCuaKhachHang = getSanLuongThangTruocCuaKhachHang(previousMonth, previousYear, item.KhachHangID);
                if (item.SanLuong <= 1 || cS.isDacBiet(item.HoaDonNuocID, month.ToString(), year.ToString()))
                {
                    danhSachHoaDonBatThuong.Add(item);
                }
                if (sanLuongThangTruocCuaKhachHang != -1 && item.SanLuong >= sanLuongThangTruocCuaKhachHang * 2)
                {
                    danhSachHoaDonBatThuong.Add(item);
                }
            }
            ViewBag.khachHang = danhSachHoaDonBatThuong.Distinct().ToList();
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
                                      join r in db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                      where i.ThangHoaDon == month && i.NamHoaDon == year && r.KhachhangID == khachhangId
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
        public void capnhatketthuc(String HoaDonID, String KhachHangID, String ngayKetThuc, String thang, String nam)
        {
            int hdID = Convert.ToInt32(HoaDonID);
            Lichsuhoadon ls = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hdID);
            ls.NgayKetThuc = ngayKetThuc;
            db.Entry(ls).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            //cập nhật ngày bắt đầu sử dụng của tháng sau luôn
            int khID = Convert.ToInt32(KhachHangID);
            int month = Convert.ToInt16(thang);
            int year = Convert.ToInt16(nam);
            Hoadonnuoc hD = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khID && p.ThangHoaDon == month && p.NamHoaDon == year);
            if (hD != null)
            {
                hD.Ngaybatdausudung = Convert.ToDateTime(ngayKetThuc);
                db.Entry(hD).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// JSON result cho phần load áp giá
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <param name="thang"></param>
        /// <param name="nam"></param>
        /// <returns></returns>

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