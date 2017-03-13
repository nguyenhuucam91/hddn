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
        public ActionResult Index()
        {
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
            ViewBag.tuyen = tuyensLs;
            ViewBag.showHoaDon = false;
            //load viewBag ngày bắt đầu            
            ViewBag.month = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
            //kiểm đinh            
            ViewBag.ngayBatDau = "";
            ViewBag.ngayKetThuc = "";
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
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            int quanHuyenID = selectedQuanHuyenID;
            //generate chi tiet hóa đơn nước tháng sau
            int nhanVienInt = String.IsNullOrEmpty(form["nhanvien"]) ? 0 : Convert.ToInt32(form["nhanvien"]);
            //gán session cho nhân viên để tiến hành thêm mới hóa đơn tháng sau cũng như lấy nhân viên cho các controller khác có thể tác động vào.          
            Session["nhanvien"] = nhanVienInt;
            //năm và tháng được chọn
            String month = form["thang"];
            String year = form["nam"];
            String selectedNhanVien = form["nhanvien"];

            int toForm = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            String selectedTuyen = form["tuyen"];
            int tuyenInt = Convert.ToInt32(selectedTuyen);
            //nếu năm tháng rỗng thì lấy năm và tháng hiện tại, nếu tuyến được chọn rỗng thì lấy là 0
            int _month = String.IsNullOrEmpty(month) ? DateTime.Now.Month : Convert.ToInt16(month);
            int _year = String.IsNullOrEmpty(year) ? DateTime.Now.Year : Convert.ToInt16(year);
            int _selectedTuyen = String.IsNullOrEmpty(selectedTuyen) ? 0 : Convert.ToInt32(selectedTuyen);
            //lấy danh sách tổ, phòng ban thuộc tổ quận huyện đó
            int phongBanID = Convert.ToInt32(Session["phongBan"]);
            List<ToQuanHuyen> toLs = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
            ViewBag.to = toLs;
            //load danh sách nhân viên thuộc tổ có phòng ban đó, lấy từ form được chọn
            List<Nhanvien> _nvLs = db.Nhanviens.OrderBy(p => p.Ten).Where(p => p.ToQuanHuyenID == toForm && (p.IsDelete == false || p.IsDelete == null) && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
            ViewBag.nhanVien = _nvLs;
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
            //lấy danh sách quận huyện để đẩy vào phần lọc chỉ số KH
            ViewBag.selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedTenChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 1);
            //dành cho người dùng
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> chiSoTieuThu = cS.filterChiSo(_month, _year, _selectedTuyen);
            ViewBag.showHoaDon = true;
            ViewBag.month = _month;
            ViewBag.year = _year;
            ViewBag.khachHang = chiSoTieuThu;
            ViewBag.nextMonth = "";

            Session["solieuTieuThuNhanvien"] = Convert.ToInt32(selectedNhanVien);
            ViewBag.selectedNhanvien = Session["solieuTieuThuNhanvien"];
            ViewBag.selectedTuyen = _selectedTuyen;
            ViewBag.selectedTo = toForm;
            //truyền 2 object sang View()
            Nhanvien nhanVienObj = db.Nhanviens.Find(nhanVienInt);
            ViewData["nhanVienObj"] = nhanVienObj;
            ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenInt);

            //kiểm tra xem nhân viên đó là trưởng phòng hay nhân viên, nếu là trưởng phòng thì cho chỉnh sửa thoải mái
            int nhanVienIDLoggedIn = Convert.ToInt32(Session["nhanVienID"]);
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
            double thue = Convert.ToInt32(cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value));
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Convert.ToInt32(dinhMuc * 0.05);
            double tongTienHoaDon = dinhMuc + thue + VAT;
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
            double tongCongCongDon = Convert.ToInt32(tongTienHoaDon + congDonHDTruoc);

            insertToLichSuSuDungNuoc(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, _TongSoTieuThu, cT.SH1.Value,
                cS.getSoTienTheoApGia("SH1").Value, cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value, cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value, cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
                cT.HC.Value, cS.getSoTienTheoApGia("HC").Value, cT.CC.Value, cS.getSoTienTheoApGia("CC").Value, cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value, cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
                5, VAT,
                obj.Tilephimoitruong.Value, thue, tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
                thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, dateStart, dateEnd);
        }

        /// <summary>
        /// Hàm tách chỉ số SH1, Sh2, 3,4 dựa theo sản lượng nhập vào
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="KHID"></param>
        /// <param name="ChiSoDau"></param>
        /// <param name="ChiSoCuoi"></param>
        /// <param name="SoKhoan"></param>
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
        public void ChinhSuaSoKhoan(int HoaDonID, int? ChiSoDau, int? ChiSoCuoi, int? TongSoTieuThu, int SoKhoan, int KHID, String dateStart, String dateEnd)
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
            double thue = Convert.ToInt32(cS.tinhThue(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value));
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Convert.ToInt32(dinhMuc * 0.05);
            double tongTienHoaDon = dinhMuc + thue + VAT;
            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - ";

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
            double tongCongCongDon = Convert.ToInt32(tongTienHoaDon + congDonHDTruoc);

            insertToLichSuSuDungNuoc(HoaDonID, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau.Value, ChiSoCuoi.Value, TongSoTieuThu.Value, cT.SH1.Value,
                cS.getSoTienTheoApGia("SH1").Value, cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value, cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value, cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
                cT.HC.Value, cS.getSoTienTheoApGia("HC").Value, cT.CC.Value, cS.getSoTienTheoApGia("CC").Value, cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value, cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
                5, VAT,
                obj.Tilephimoitruong.Value, thue,
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
                                    KHID = r.KhachhangID
                                }).OrderBy(p => p.ThuTuDoc).ToList();
            ViewBag.chiSoTieuThu = chiSoTieuThu;
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
        public ActionResult Apgiadacbiet(int? id, int month, int year)
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
        public ActionResult Nhapgiadacbiet(int? id, FormCollection form)
        {
            int SH1 = ChiSo.checkChiSoNull(form["SH1"]);
            int SH2 = ChiSo.checkChiSoNull(form["SH2"]);
            int SH3 = ChiSo.checkChiSoNull(form["SH3"]);
            int SH4 = ChiSo.checkChiSoNull(form["SH4"]);
            int HC = ChiSo.checkChiSoNull(form["HC"]);
            int CC = ChiSo.checkChiSoNull(form["CC"]);
            int SX = ChiSo.checkChiSoNull(form["SX"]);
            int KD = ChiSo.checkChiSoNull(form["KD"]);
            String startDate = form["startDate"];
            String endDate = form["endDate"];

            int Sum = SH1 + SH2 + SH3 + SH4 + HC + CC + SX + KD;
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
                //không có thì cập nhật
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
            Chitiethoadonnuoc _chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == id);
            _chiTiet.Chisomoi = 0;
            _chiTiet.KDDV = 0;
            _chiTiet.SH1 = 0;
            _chiTiet.SH2 = 0;
            _chiTiet.SH3 = 0;
            _chiTiet.SH4 = 0;
            _chiTiet.CC = 0;
            _chiTiet.HC = 0;
            _chiTiet.SXXD = 0;
            db.Entry(_chiTiet).State = System.Data.Entity.EntityState.Modified;
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
            hD.themMoiSoTienPhaiNop(id.Value);
            //thêm vào bảng lịch sử
            Khachhang obj = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            Chitiethoadonnuoc cT = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == id);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == obj.TuyenKHID);
            int _month = hoaDonDacBiet.ThangHoaDon.Value;
            int _year = hoaDonDacBiet.NamHoaDon.Value;
            int ChiSoDau = cT.Chisocu.Value;
            int ChiSoCuoi = cT.Chisomoi.Value;
            //Giá 
            double thue = Convert.ToInt32(cS.tinhThue(id.Value, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value, obj.Tilephimoitruong.Value));
            double dinhMuc = cS.tinhTongTienTheoDinhMuc(id.Value, cT.SH1.Value, cT.SH2.Value, cT.SH3.Value, cT.SH4.Value, cT.HC.Value, cT.CC.Value, cT.KDDV.Value, cT.SXXD.Value);
            double VAT = Convert.ToInt32(dinhMuc * 0.05);
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
            double tongCongCongDon = Convert.ToInt32(tongTienHoaDon + congDonHDTruoc);
            //thu ngân
            String thuNgan = obj.TTDoc + "/" + tuyenKH.Matuyen + " - ";

            insertToLichSuSuDungNuoc(id.Value, _month, _year, obj.Ten, obj.Diachi, obj.Masothue, obj.MaKhachHang, obj.TuyenKHID.Value, obj.Sohopdong, ChiSoDau, ChiSoCuoi, Sum, cT.SH1.Value,
                cS.getSoTienTheoApGia("SH1").Value, cT.SH2.Value, cS.getSoTienTheoApGia("SH2").Value, cT.SH3.Value, cS.getSoTienTheoApGia("SH3").Value, cT.SH4.Value, cS.getSoTienTheoApGia("SH4").Value,
                cT.HC.Value, cS.getSoTienTheoApGia("HC").Value, cT.CC.Value, cS.getSoTienTheoApGia("CC").Value, cT.SXXD.Value, cS.getSoTienTheoApGia("SX-XD").Value, cT.KDDV.Value, cS.getSoTienTheoApGia("KDDV").Value,
                5, VAT,
                obj.Tilephimoitruong.Value, thue,
                tongTienHoaDon, ConvertMoney.So_chu(tongTienHoaDon),
                db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai2 + "<br/>" + db.Quanhuyens.Find(obj.QuanhuyenID).DienThoai3,
                thuNgan, obj.TuyenKHID.Value, obj.TTDoc.Value, tongCongCongDon, startDate, endDate);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Thêm mới record vào lịch sử sử dụng nước
        /// </summary>
        /// <param name="HoaDonID"></param>
        /// <param name="thangHoaDon"></param>
        /// <param name="namHoaDon"></param>
        /// <param name="tenKH"></param>
        /// <param name="diaChi"></param>
        /// <param name="MST"></param>
        /// <param name="maKH"></param>
        /// <param name="TuyenKHID"></param>
        /// <param name="soHD"></param>
        /// <param name="chiSoCu"></param>
        /// <param name="ChiSoMoi"></param>
        /// <param name="TongTieuThu"></param>
        /// <param name="SH1"></param>
        /// <param name="SH1Price"></param>
        /// <param name="SH2"></param>
        /// <param name="SH2Price"></param>
        /// <param name="SH3"></param>
        /// <param name="SH3Price"></param>
        /// <param name="SH4"></param>
        /// <param name="SH4Price"></param>
        /// <param name="HC"></param>
        /// <param name="doubleHCPrice"></param>
        /// <param name="CC"></param>
        /// <param name="CCPrice"></param>
        /// <param name="SX"></param>
        /// <param name="SXPrice"></param>
        /// <param name="KD"></param>
        /// <param name="KDPrice"></param>
        /// <param name="Thue"></param>
        /// <param name="TienThueVAT"></param>
        /// <param name="TileBVMT"></param>
        /// <param name="BVMTPrice"></param>
        /// <param name="TongCong"></param>
        /// <param name="bangChu"></param>
        /// <param name="TTVoOng"></param>
        /// <param name="ThuNgan"></param>
        /// <param name="tuyen"></param>
        /// <param name="TTDoc"></param>
        /// <param name="chiSoCongDon"></param>
        /// <param name="ngayBatDau"></param>
        /// <param name="ngayKetThuc"></param>

        public void insertToLichSuSuDungNuoc(int HoaDonID, int thangHoaDon, int namHoaDon, String tenKH, String diaChi, String MST, String maKH, int TuyenKHID, String soHD,
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
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
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
        }

        /// <summary>
        /// Dành cho khách hàng áp giá tổng hợp, tách chỉ số giá tổng hợp ra.
        /// </summary>
        public void tachSoTongHop(int HoaDonID, int cachTinh, int KhachHangID, double SanLuong)
        {
            // double[] dayTongHop;            
            //lấy cấu hình áp giá tổng hợp
            List<Apgiatonghop> ls = db.Apgiatonghops.Where(p => p.KhachhangID == KhachHangID).OrderByDescending(p => p.IDLoaiApGia).ToList();

            //lấy chi tiết hóa đơn để lưu vào bảng
            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            if (chiTiet != null)
            {
                //reset chỉ số chi tiết trước khi tách số tổng hợp            
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
                    double TongSanLuongTongHop = ls.Sum(p => p.SanLuong).Value;
                    foreach (var item in ls)
                    {
                        //nếu sản lượng = 0 thì giáng vào nó
                        if (item.SanLuong == 0)
                        {
                            //nếu sản lượng  = 0 rơi vào sinh hoạt
                            if (item.IDLoaiApGia == KhachHang.SINHHOAT)
                            {

                                int _TongSoTieuThu = Convert.ToInt32(SanLuong) - Convert.ToInt32(TongSanLuongTongHop);
                                int soNhanKhau = kHHelper.getSoNhanKhau(KhachHangID);
                                int soHo = kHHelper.getSoHo(KhachHangID);
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;

                                double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
                                double SH1, SH2, SH3, SH4;
                                SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;
                                //40-16 = 24<=20 ? 4:20
                                //33-16 = 17<=20 ? 17:20
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

                                chiTiet.SH1 = SH1;
                                chiTiet.SH2 = SH2;
                                chiTiet.SH3 = SH3;
                                chiTiet.SH4 = SH4;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SANXUAT)
                            {
                                chiTiet.SXXD = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.KINHDOANHDICHVU)
                            {
                                chiTiet.KDDV = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.COQUANHANHCHINH)
                            {
                                chiTiet.HC = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.DONVISUNGHIEP)
                            {
                                chiTiet.CC = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH1)
                            {
                                chiTiet.SH1 = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH2)
                            {
                                chiTiet.SH2 = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH3)
                            {
                                chiTiet.SH3 = SanLuong - TongSanLuongTongHop;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH4)
                            {
                                chiTiet.SH4 = SanLuong - TongSanLuongTongHop;
                            }

                            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                        //nếu không bị dồn số thì tính như bt, i.e. số được định nghĩa thay vì số 0
                        else
                        {

                            if (item.IDLoaiApGia == KhachHang.SINHHOAT)
                            {

                                int _TongSoTieuThu = Convert.ToInt32(item.SanLuong.Value);
                                int soNhanKhau = kHHelper.getSoNhanKhau(KhachHangID);
                                int soHo = kHHelper.getSoHo(KhachHangID);
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;

                                double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
                                double SH1, SH2, SH3, SH4;
                                SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;
                                //40-16 = 24<=20 ? 4:20
                                //33-16 = 17<=20 ? 17:20
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
                            }
                            else if (item.IDLoaiApGia == KhachHang.SANXUAT)
                            {
                                chiTiet.SXXD = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.KINHDOANHDICHVU)
                            {
                                chiTiet.KDDV = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.COQUANHANHCHINH)
                            {
                                chiTiet.HC = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.DONVISUNGHIEP)
                            {
                                chiTiet.CC = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH1)
                            {
                                chiTiet.SH1 = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH2)
                            {
                                chiTiet.SH2 = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH3)
                            {
                                chiTiet.SH3 = item.SanLuong;
                            }
                            else if (item.IDLoaiApGia == KhachHang.SH4)
                            {
                                chiTiet.SH4 = item.SanLuong;
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
                    int itemLeft = ls.Count();
                    foreach (var item in ls)
                    {
                        //ưu tiên chia CC, HC ...trước
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
                                if (itemLeft == 1)
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
                                if (itemLeft == 1)
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
                                if (itemLeft == 1)
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
                                int soNhanKhau = kHHelper.getSoNhanKhau(KhachHangID);
                                int soHo = kHHelper.getSoHo(KhachHangID);
                                int dinhMucCoSo = db.Apgias.Find(item.IDLoaiApGia).Denmuc.Value;

                                double dinhMucTungNha = cS.getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
                                double SH1, SH2, SH3, SH4;
                                SH1 = _TongSoTieuThu <= dinhMucTungNha ? _TongSoTieuThu : dinhMucTungNha;
                                //40-16 = 24<=20 ? 4:20
                                //33-16 = 17<=20 ? 17:20
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
                                    SH2 = _TongSoTieuThu - SH1 <= (dinhMucSH2 - dinhMucSH1) * soHo ? _TongSoTieuThu - SH1 : (dinhMucSH2 - dinhMucSH1) * soHo;
                                }

                                if (_TongSoTieuThu - SH1 - SH2 <= 0)
                                {
                                    SH3 = 0;
                                }
                                else
                                {
                                    SH3 = _TongSoTieuThu - SH1 - SH2 <= (dinhMucSH3 - dinhMucSH2) * soHo ? _TongSoTieuThu - SH1 - SH2 : (dinhMucSH3 - dinhMucSH2) * soHo;
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
                            }
                            db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        itemLeft--;
                    }
                }//end cach tinh = 1 
            }
        }//end ham

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
            return RedirectToAction("Index");
        }

        /// <summary>
        /// load hóa đơn có chỉ số không bình thường lên, giống vs hàm load danh sách hóa đơn, chỉ khác là load danh sách hóa đơn có sản lượng <code><=1</code>
        /// </summary>
        /// <returns></returns>

        public ActionResult LoadKhongBt(int tuyenID, int month, int year, int nhanvienInt)
        {
            ViewBag.year = year;
            ViewBag.month = month;
            var danhSachHoaDonBatThuong = (from i in db.Hoadonnuocs
                                           join r in db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                           join m in db.Chitiethoadonnuocs on i.HoadonnuocID equals m.HoadonnuocID
                                           where i.ThangHoaDon == month && i.NamHoaDon == year && r.TuyenKHID == tuyenID
                                           && (r.Tinhtrang == 0 || r.Tinhtrang == null) && (r.IsDelete == false) && (i.Tongsotieuthu <= 1)
                                           orderby r.TTDoc
                                           select new
                                           {
                                               HoaDonNuocID = i.HoadonnuocID,
                                               KhachHangID = r.KhachhangID,
                                               MaKhachHang = r.MaKhachHang,
                                               TenKhachHang = r.Ten,
                                               SoHo = r.Soho,
                                               SoKhau = r.Sonhankhau,
                                               ChiSoCu = m.Chisocu,
                                               ChiSoMoi = m.Chisomoi,
                                               ColID = i.HoadonnuocID,
                                               SanLuong = i.Tongsotieuthu,
                                               ThuTuDoc = r.TTDoc
                                           }).ToList();
            ViewBag.khachHang = danhSachHoaDonBatThuong;
            ViewBag.currentDate = String.Concat(DateTime.Now.Day, '/', DateTime.Now.Month);
            ViewBag.nextMonth = String.Concat(DateTime.Now.Day, '/', DateTime.Now.Month + 1 > 12 ? 1 : DateTime.Now.Month + 1);
            ViewData["nhanVienObj"] = db.Nhanviens.Find(nhanvienInt);
            ViewData["tuyenObj"] = db.Tuyenkhachhangs.Find(tuyenID);
            return View();
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