using HoaDonNuocHaDong;
using HoaDonNuocHaDong.Config;
using HoaDonNuocHaDong.Helper;
using HvitFramework;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using HoaDonNuocHaDong.Models.SoLieuTieuThu;
using System.Data;

namespace HoaDonHaDong.Helper
{
    public class ChiSo
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        TimeHelper time = new TimeHelper();
        /// <summary>
        /// Hàm để tính chỉ số
        /// </summary>
        /// <param name="nguoi">Số nhân khẩu</param>
        /// <param name="khau">Số hộ khẩu</param>
        /// <param name="nacthang">đến mức trong bảng áp giá</param>
        /// <returns></returns>
        public static double tinhdv(int nguoi, int khau, int nacthang, double chiSo)
        {
            if (chiSo < nacthang * khau)
            {
                return chiSo;
            }
            else
            {
                //tinh bien he so bang gia tri cua nac thang chia 4, ep kieu ve double; chia theo nấc thang, mỗi nấc thang có hệ số khác nhau
                double heso = (double)nacthang / 4;
                //neu so nhan khau nho hon hoac bang 4 thi tra ve chinh gia tri cua nac thang
                //day chinh la truong hop mac dinh cho 1 khau
                if (nguoi <= 4)
                    return khau * nacthang;
                //neu so nguoi chia 4 ma lon hon so khau thi tra ve gia tri chi so cua nac thang = he so x nac thang
                else
                {
                    if ((nguoi / 4) >= khau)
                        return khau * nacthang;
                    //neu so nguoi chi 4 <= so khau
                    //tinh ra so khau chan = so nguoi /4 va so nguoi le ra = nguoi%4
                    //tra ve gia tri cua nac thang = so khau chan x nac thang + so nguoi le x he so
                    else
                    {
                        int soKhauChan = (int)(nguoi / 4);
                        double soNguoiDu = (nguoi % 4);
                        double SH1 = soKhauChan * nacthang + soNguoiDu * 0.25 * nacthang;
                        return SH1;
                    }
                }
            }
        }


        /// <summary>
        /// Lấy định mức SH1, SH2, SH3,SH4,...cho từng nhà
        /// </summary>
        /// <param name="hoKhau"></param>
        /// <param name="nhanKhau"></param>
        /// <param name="dinhMucCoSo"></param>
        /// <returns></returns>
        public double getDinhMucTungNha(int hoKhau, int nhanKhau, int dinhMucCoSo)
        {
            if (hoKhau == 1)
            {
                return dinhMucCoSo;
            }
            //nếu hộ khẩu > 1
            else
            {
                int soKhauChuan = (hoKhau - 1) * 4;
                if (nhanKhau > soKhauChuan)
                {
                    int soNgDu = nhanKhau - soKhauChuan;
                    if (soNgDu >= 4)
                    {
                        soNgDu = 4;
                    }
                    return (hoKhau - 1) * dinhMucCoSo + soNgDu * 2.5;
                }
            }
            return 0;
        }


        /// <summary>
        /// Lấy chỉ số tháng truoc của khách hàng 
        /// </summary>
        /// <param name="khachHangID"></param>
        /// <returns></returns>
        public static int getChiSoThang(String month, String year, int khachHangID)
        {
            var hoaDonNuoc = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachHangID && p.ThangHoaDon.ToString() == month && p.NamHoaDon.ToString() == year);
            if (hoaDonNuoc != null)
            {
                return hoaDonNuoc.Chitiethoadonnuocs.FirstOrDefault().Chisocu.Value;
            }
            return 0;
        }

        /// <summary>
        /// Lọc thông tin nhập chỉ số theo tháng và năm
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> filterChiSo(int month, int year, int? tuyenKHID)
        {
            ControllerBase<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> cb = new ControllerBase<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc>();
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> _hoaDonNuoc = cb.Query("DanhSachHoaDonsTheoThangNam",
                       new SqlParameter("@d1", month),
                       new SqlParameter("@d2", year),
                       new SqlParameter("@d3", tuyenKHID.Value)
                       );

            return _hoaDonNuoc;
        }

        /// <summary>
        /// Kiểm tra xem chỉ số có NULL hay ko, nếu null thì trả về 0
        /// </summary>
        /// <param name="chiSo"></param>
        /// <returns></returns>
        public static double checkChiSoNull(String chiSo)
        {
            if (String.IsNullOrEmpty(chiSo))
            {
                return 0;
            }
            return Convert.ToDouble(chiSo);
        }

        /// <summary>
        /// Lấy số tiền theo áp giá
        /// </summary>
        /// <param name="ApGiaID"></param>
        /// <returns></returns>
        public double? getSoTienTheoApGia(String tenApGia)
        {
            HoaDonHaDongEntities thisDB = new HoaDonHaDongEntities();
            switch (tenApGia)
            {
                case "SH1":
                    Apgia _apGiaSH1 = thisDB.Apgias.FirstOrDefault(p => p.Ten == "SH1");
                    if (_apGiaSH1 != null)
                    {
                        return _apGiaSH1.Gia;
                    }
                    break;

                case "SH2":
                    Apgia _apGiaSH2 = thisDB.Apgias.FirstOrDefault(p => p.Ten == "SH2");
                    if (_apGiaSH2 != null)
                    {
                        return _apGiaSH2.Gia;
                    }
                    break;
                case "SH3":
                    Apgia _apGiaSH3 = thisDB.Apgias.FirstOrDefault(p => p.Ten == "SH3");
                    if (_apGiaSH3 != null)
                    {
                        return _apGiaSH3.Gia;
                    }
                    break;
                case "SH4":
                    Apgia _apGiaSH4 = thisDB.Apgias.FirstOrDefault(p => p.Ten == "SH4");
                    if (_apGiaSH4 != null)
                    {
                        return _apGiaSH4.Gia;
                    }
                    break;

                //khối kinh doanh
                case "HC":
                    Apgia HC = thisDB.Apgias.FirstOrDefault(p => p.Ten == "HC");
                    if (HC != null)
                    {
                        return HC.Gia;
                    }
                    break;

                case "CC":
                    Apgia CC = thisDB.Apgias.FirstOrDefault(p => p.Ten == "CC");
                    if (CC != null)
                    {
                        return CC.Gia;
                    }
                    break;
                case "SX-XD":
                    Apgia SXXD = thisDB.Apgias.FirstOrDefault(p => p.Ten == "SX-XD");
                    if (SXXD != null)
                    {
                        return SXXD.Gia;
                    }
                    break;
                case "KDDV":
                    Apgia KDDV = thisDB.Apgias.FirstOrDefault(p => p.Ten == "KDDV");
                    if (KDDV != null)
                    {
                        return KDDV.Gia;
                    }
                    break;
            }
            return 0;
        }

        public double tinhTongTienHoaDon(int hoaDonNuocID, double SH1, double SH2, double SH3, double SH4,
            double HC, double CC, double KD, double SX, int thangHoaDon, int namHoaDon, int tiLePhiBVMT)
        {
            double tongTienDinhMuc = tinhTongTienTheoDinhMuc(hoaDonNuocID, SH1, SH2, SH3, SH4, HC, CC, KD, SX);
            return tongTienDinhMuc + (tongTienDinhMuc * 5 / 100) + (tongTienDinhMuc * tiLePhiBVMT / 100);

        }

        /// <summary>
        /// Tính thuế BVMT
        /// </summary>
        /// <param name="hoaDonNuocID"></param>
        /// <param name="SH1"></param>
        /// <param name="SH2"></param>
        /// <param name="SH3"></param>
        /// <param name="SH4"></param>
        /// <param name="HC"></param>
        /// <param name="CC"></param>
        /// <param name="KD"></param>
        /// <param name="SX"></param>
        /// <param name="tileBVMT"></param>
        /// <returns></returns>

        public double tinhThue(int hoaDonNuocID, double SH1, double SH2, double SH3, double SH4, double HC, double CC, double KD, double SX, double? tileBVMT)
        {
            double dinhMuc = tinhTongTienTheoDinhMuc(hoaDonNuocID, SH1, SH2, SH3, SH4, HC, CC, KD, SX);
            double thueBVMT = dinhMuc * tileBVMT.Value / 100;
            return Math.Round(thueBVMT, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Tính tổng tiền theo định mức giá
        /// </summary>
        /// <param name="hoaDonNuocID"></param>
        /// <param name="SH1"></param>
        /// <param name="SH2"></param>
        /// <param name="SH3"></param>
        /// <param name="SH4"></param>
        /// <param name="HC"></param>
        /// <param name="CC"></param>
        /// <param name="KD"></param>
        /// <param name="SX"></param>
        /// <returns></returns>
        public double tinhTongTienTheoDinhMuc(int hoaDonNuocID, double SH1, double SH2, double SH3, double SH4, double HC, double CC, double KD, double SX)
        {
            //giá chuẩn
            //Áp giá mặc định
            double SH1Price = getSoTienTheoApGia("SH1").Value;
            double SH2Price = getSoTienTheoApGia("SH2").Value;
            double SH3Price = getSoTienTheoApGia("SH3").Value;
            double SH4Price = getSoTienTheoApGia("SH4").Value;
            double HCPrice = getSoTienTheoApGia("HC").Value;
            double CCPrice = getSoTienTheoApGia("CC").Value;
            double SXPrice = getSoTienTheoApGia("SX-XD").Value;
            double KDPrice = getSoTienTheoApGia("KDDV").Value;

            double SH1Total = SH1 * SH1Price;
            double SH2Total = SH2 * SH2Price;
            double SH3Total = SH3 * SH3Price;
            double SH4Total = SH4 * SH4Price;
            double HCTotal = HC * HCPrice;
            double CCTotal = CC * CCPrice;
            double KDTotal = KD * KDPrice;
            double SXTotal = SX * SXPrice;

            double Sum = SH1Total + SH2Total + SH3Total + SH4Total + HCTotal + CCTotal + KDTotal + SXTotal;
            return Math.Floor(Sum);
        }

        /// <summary>
        /// Viết tắt loại giá
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string vietTatLoaiGia(dynamic input)
        {
            if (input == KhachHang.SINHHOAT) return "SH";
            else if (input == KhachHang.SANXUAT) return "SX";
            else if (input == KhachHang.KINHDOANHDICHVU) return "KD";
            else if (input == KhachHang.DONVISUNGHIEP) return "CC";
            else if (input == KhachHang.TONGHOP) return "TH";
            return "DB";
        }

        /// <summary>
        /// Kiểm tra xem hóa đơn đó có phải là hóa đơn áp giá đặc biệt hay không
        /// </summary>
        /// <returns></returns>
        public bool isDacBiet(int hoaDonNuocID, String month, String year)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            var isDacBiet = (from dacbiet in _db.ApGiaDacBiets
                             join hoadon in _db.Hoadonnuocs on dacbiet.HoaDonNuocID.Value equals hoadon.HoadonnuocID
                             where dacbiet.HoaDonNuocID == hoaDonNuocID && hoadon.ThangHoaDon.ToString() == month && hoadon.NamHoaDon.ToString() == year
                             select new { }).Count();
            if (isDacBiet > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra xem hóa đơn đã chốt hay chưa, nếu chưa chốt thì vẫn để mở
        /// </summary>
        /// <param name="hoadonID"></param>
        /// <returns></returns>
        public bool isChotHoaDon(int hoadonID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            Hoadonnuoc hoaDon = _db.Hoadonnuocs.Find(hoadonID);
            if (hoaDon != null)
            {
                bool? ttChot = hoaDon.Trangthaichot;
                if (ttChot == null || ttChot == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Cập nhật trạng thái chốt hóa đơn cho các khách hàng
        /// </summary>
        /// <param name="tuyenID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>

        public void capNhatTrangThaiChotHoaDon(int tuyenID, int month, int year)
        {
            DateTime startDate = getThangNamGanNhatThuocHoaDon(tuyenID, month, year);
            DateTime endDate = new DateTime(year, month, 1);
            DateTime[] monthYearsBetween = time.getMonthYearBetweenDates(startDate, endDate);
            string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                foreach (var monthYearBetween in monthYearsBetween)
                {
                    command.CommandText = "UPDATE A SET [Trangthaichot] = 1 FROM [dbo].[Hoadonnuoc] A JOIN [dbo].[Khachhang] B on A.KhachhangID = B.KhachhangID WHERE A.ThangHoaDon=@thang AND A.NamHoaDon =@nam AND B.TuyenKHID=@tuyen";
                    command.Parameters.AddWithValue("@thang", monthYearBetween.Month);
                    command.Parameters.AddWithValue("@nam", monthYearBetween.Year);
                    command.Parameters.AddWithValue("@tuyen", tuyenID);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                connection.Close();
            }
        }

        public void capnhatTrangThaiDanhSachHoaDonBiHuyThuocTuyen(int tuyenID, int month, int year)
        {
            string connectionString = DatabaseConfig.getConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "UPDATE A SET [Trangthaicapnhathuy] = 1 FROM [dbo].[Hoadonnuocbihuy] A " +
                "JOIN [dbo].[Hoadonnuoc] B on A.HoadonnuocID = B.HoadonnuocID " +
                "JOIN [dbo].[Khachhang] C on B.KhachhangID = C.KhachhangID " +
                "WHERE B.ThangHoaDon=@thang AND B.NamHoaDon = @nam AND C.TuyenKHID=@tuyen";
                command.Parameters.AddWithValue("@thang", month);
                command.Parameters.AddWithValue("@nam", year);
                command.Parameters.AddWithValue("@tuyen", tuyenID);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public int getSoHoaDonTrongDanhSachKhongBinhThuong(int month, int year, int tuyenKHID, int hoaDonID)
        {
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> danhSachHoaDon = filterChiSo(month, year, tuyenKHID);
            int soHoaDon = 1;
            foreach (var item in danhSachHoaDon)
            {
                if (item.HoaDonNuocID == hoaDonID)
                {
                    return soHoaDon;
                }
                soHoaDon++;
            }
            return soHoaDon;
        }

        public void generateChiSoFromNearestMonth(int currentMonth, int currentYear, int nhanVienId, int tuyenKHID)
        {
            DateTime thangNamGanNhat = getThangNamGanNhatThuocHoaDon(tuyenKHID, currentMonth, currentYear);
            int previousMonth = thangNamGanNhat.Month;
            int previousYear = thangNamGanNhat.Year;

            if (previousYear != 1)
            {
                var hoaDonNuocsThangTruoc = getHoaDonThang(previousMonth, previousYear, tuyenKHID);
                var hoaDonNuocsHienTai = getHoaDonThang(currentMonth, currentYear, tuyenKHID);

                int countHoaDonsThangTruoc = hoaDonNuocsThangTruoc.Count();
                int countHoaDonsThangHienTai = hoaDonNuocsHienTai.Count();

                if (countHoaDonsThangTruoc != 0)
                {
                    foreach (var hoaDonNuocThangTruoc in hoaDonNuocsThangTruoc)
                    {
                        HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc hoaDonNuoc = hoaDonNuocsHienTai.FirstOrDefault(p => p.Thang == currentMonth && p.Nam == currentYear && p.KhachHangID == hoaDonNuocThangTruoc.KhachHangID);
                        if (hoaDonNuoc == null)
                        {
                            Hoadonnuoc hoaDonThangHienTai = new Hoadonnuoc();
                            hoaDonThangHienTai.ThangHoaDon = currentMonth;
                            hoaDonThangHienTai.NamHoaDon = currentYear;
                            if (hoaDonNuocThangTruoc.NgayKetThucSuDung == null || hoaDonNuocThangTruoc.NgayKetThucSuDung == DateTime.MinValue)
                            {
                                hoaDonThangHienTai.Ngaybatdausudung = hoaDonNuocThangTruoc.NgayBatDauSuDung;
                            }
                            else
                            {
                                hoaDonThangHienTai.Ngaybatdausudung = hoaDonNuocThangTruoc.NgayKetThucSuDung;
                            }

                            hoaDonThangHienTai.KhachhangID = hoaDonNuocThangTruoc.KhachHangID;
                            hoaDonThangHienTai.NhanvienID = nhanVienId;
                            hoaDonThangHienTai.Trangthaixoa = false;
                            hoaDonThangHienTai.Trangthaithu = false;
                            hoaDonThangHienTai.Trangthaiin = false;
                            hoaDonThangHienTai.Trangthaichot = false;
                            db.Hoadonnuocs.Add(hoaDonThangHienTai);
                            db.SaveChanges();
                            //Thêm chi tiết hóa đơn nước tháng sau
                            Chitiethoadonnuoc chiTiet = new Chitiethoadonnuoc();
                            chiTiet.HoadonnuocID = hoaDonThangHienTai.HoadonnuocID;
                            chiTiet.Chisocu = hoaDonNuocThangTruoc.ChiSoMoi;
                            db.Chitiethoadonnuocs.Add(chiTiet);
                            db.SaveChanges();
                        }
                        
                    }
                }
            }
        }

        public List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> getHoaDonThang(int month, int year, int tuyenKHID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            var hoaDonNuocs = (from i in _db.Hoadonnuocs
                               join kH in _db.Khachhangs on i.KhachhangID equals kH.KhachhangID
                               join cT in _db.Chitiethoadonnuocs on i.HoadonnuocID equals cT.HoadonnuocID
                               where i.ThangHoaDon == month && i.NamHoaDon == year && kH.TuyenKHID == tuyenKHID
                               select new HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc
                               {
                                   NgayKetThucSuDung = i.Ngayketthucsudung,
                                   KhachHangID = i.KhachhangID.Value,
                                   ChiSoMoi = cT.Chisomoi,
                                   HoaDonNuocID = i.HoadonnuocID,
                                   SanLuong = i.Tongsotieuthu,
                                   NgayBatDauSuDung = i.Ngaybatdausudung,
                                   Thang = month,
                                   Nam = year,
                               });
            return hoaDonNuocs.ToList();

        }

        public DateTime getThangNamGanNhatThuocHoaDon(int tuyenKHID, int currentMonth, int currentYear)
        {
            DateTime ngayThangHoaDon = new DateTime(1, 1, 1);
            DateTime currentTime = new DateTime(currentYear, currentMonth, 1);
            DateTime nearestDate = new DateTime(1, 1, 1);
            var hoaDonThangGanNhat = (from i in db.Hoadonnuocs
                                      join kH in db.Khachhangs on i.KhachhangID equals kH.KhachhangID
                                      join cT in db.Chitiethoadonnuocs on i.HoadonnuocID equals cT.HoadonnuocID
                                      where kH.TuyenKHID == tuyenKHID
                                      select new HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc
                                      {
                                          Thang = i.ThangHoaDon.Value,
                                          Nam = i.NamHoaDon.Value,
                                          HoaDonNuocID = i.HoadonnuocID
                                      }
                                   ).Distinct().ToList();
            if (hoaDonThangGanNhat.Count > 0)
            {
                HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc obj = hoaDonThangGanNhat.First();
                nearestDate = new DateTime(obj.Nam, obj.Thang, 1);
                foreach (var item in hoaDonThangGanNhat)
                {
                    ngayThangHoaDon = new DateTime(item.Nam, item.Thang, 1);

                    if (DateTime.Compare(ngayThangHoaDon, nearestDate) > 0 && DateTime.Compare(ngayThangHoaDon, currentTime) < 0)
                    {
                        nearestDate = ngayThangHoaDon;
                    }
                }

                return new DateTime(nearestDate.Year, nearestDate.Month, 1);
            }
            return ngayThangHoaDon;
        }

        public List<DanhSachKhachHangCoSanLuongBatThuong> loadDanhSachSanLuongBatThuong(int previousMonth, int previousYear, int month, int year, int tuyenID)
        {
            var danhSachHoaDonBatThuong = new List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc>();
            ControllerBase<DanhSachKhachHangCoSanLuongBatThuong> cB = new ControllerBase<DanhSachKhachHangCoSanLuongBatThuong>();
            var danhSachHoaDon = cB.Query("DanhSachKhachHangCoSanLuongBatThuong",
                new SqlParameter("@prevMonth", previousMonth),
                new SqlParameter("@prevYear", previousYear),
                new SqlParameter("@currMonth", month),
                new SqlParameter("@currYear", year),
                new SqlParameter("@tuyen", tuyenID)).ToList();
            //load danh sách khách hàng có áp giá đặc biệt
           

            return danhSachHoaDon;
        }

        public void capNhatTrangThaiChotTuyen(int tuyenKHID, int month, int year)
        {
            DateTime thangNamGanNhat = getThangNamGanNhatThuocHoaDon(tuyenKHID, month, year);
            DateTime thangNamHienTai = new DateTime(year, month, 1);
            DateTime[] monthsYears = time.getMonthYearBetweenDates(thangNamGanNhat, thangNamHienTai);
            foreach (var monthYear in monthsYears)
            {
                var isChot = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenKHID && p.Thang == monthYear.Month && p.Nam == monthYear.Year);
                if (isChot == null)
                {
                    TuyenDuocChot tuyenChot = new TuyenDuocChot();
                    tuyenChot.TuyenKHID = tuyenKHID;
                    tuyenChot.Nam = year;
                    tuyenChot.Thang = month;
                    tuyenChot.TrangThaiChot = true;
                    tuyenChot.TrangThaiTinhTien = false;
                    tuyenChot.NgayChot = DateTime.Now;
                    db.TuyenDuocChots.Add(tuyenChot);
                    db.SaveChanges();
                }
            }
        }

        public double[] tachChiSoSanLuong(int _TongSoTieuThu, int _loaiApGia, int soHo, int soNhanKhau)
        {
            double[] chiSoSuDung = new double[8];
            double SH1 = 0;
            double SH2 = 0;
            double SH3 = 0;
            double SH4 = 0;
            double CC = 0;
            double HC = 0;
            double SXXD = 0;
            double KDDV = 0;
            if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.SINHHOAT)
            {
                int dinhMucCoSo = 10;
                double dinhMucTungNha = getDinhMucTungNha(soHo, soNhanKhau, dinhMucCoSo);
               
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

            }           
            
            //loại khách hàng: doanh nghiệp - tính theo số kinh doanh, CC,...
            else
            {
                //nếu khách hàng là kinh doanh 
                if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.KINHDOANHDICHVU)
                {
                    KDDV = _TongSoTieuThu;
                }
                //Sản xuất
                else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.SANXUAT)
                {
                    SXXD = _TongSoTieuThu;
                }
                //đơn vị sự nghiệp
                else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.DONVISUNGHIEP)
                {
                    CC = _TongSoTieuThu;
                }
                //kinh doanh dịch vụ
                else if (_loaiApGia == HoaDonNuocHaDong.Helper.KhachHang.COQUANHANHCHINH)
                {
                    HC = _TongSoTieuThu;
                }
            }

            chiSoSuDung[0] = SH1;
            chiSoSuDung[1] = SH2;
            chiSoSuDung[2] = SH3;
            chiSoSuDung[3] = SH4;
            chiSoSuDung[4] = CC;
            chiSoSuDung[5] = HC;
            chiSoSuDung[6] = SXXD;
            chiSoSuDung[7] = KDDV;
            return chiSoSuDung;
        }


    }
}