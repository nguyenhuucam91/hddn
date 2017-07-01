using HoaDonNuocHaDong;
using HoaDonNuocHaDong.Config;
using HoaDonNuocHaDong.Helper;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace HoaDonHaDong.Helper
{
    public class ChiSo
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
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
        /// Lấy chỉ số SH theo cấp, ví dụ cấp 1,2,3,4 tương ứng với SH1,SH2,SH3,SH4
        /// </summary>
        /// <param name="level">SH1,2,3,4</param>
        /// <returns></returns>
        public static int? getChiSoSHTheoCap(String level)
        {
            switch (level)
            {
                case "SH1":
                    Apgia _apGiaSH1 = db.Apgias.FirstOrDefault(p => p.Ten == "SH1");
                    if (_apGiaSH1 != null)
                    {
                        return _apGiaSH1.Denmuc;
                    }
                    break;
                case "SH2":
                    Apgia _apGiaSH2 = db.Apgias.FirstOrDefault(p => p.Ten == "SH2");
                    if (_apGiaSH2 != null)
                    {
                        return _apGiaSH2.Denmuc;
                    }
                    break;
                case "SH3":
                    Apgia _apGiaSH3 = db.Apgias.FirstOrDefault(p => p.Ten == "SH3");
                    if (_apGiaSH3 != null)
                    {
                        return _apGiaSH3.Denmuc;
                    }
                    break;

            }

            return 0;
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

        /// <summary>
        /// Tính tổng tiền của toàn hóa đơn
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
        /// <param name="thangHoaDon"></param>
        /// <param name="namHoaDon"></param>
        /// <param name="VAT"></param>
        /// <param name="tiLePhiBVMT"></param>
        /// <returns></returns>
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

            double SH1Total = 0;
            double SH2Total = 0;
            double SH3Total = 0;
            double SH4Total = 0;
            double HCTotal = 0;
            double CCTotal = 0;
            double KDTotal = 0;
            double SXTotal = 0;
            double Sum = 0;

            SH1Total = SH1 * SH1Price;
            SH2Total = SH2 * SH2Price;
            SH3Total = SH3 * SH3Price;
            SH4Total = SH4 * SH4Price;
            HCTotal = HC * HCPrice;
            CCTotal = CC * CCPrice;
            KDTotal = KD * KDPrice;
            SXTotal = SX * SXPrice;

            Sum = SH1Total + SH2Total + SH3Total + SH4Total + HCTotal + CCTotal + KDTotal + SXTotal;
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
            string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "UPDATE A SET [Trangthaichot] = 1 FROM [dbo].[Hoadonnuoc] A JOIN [dbo].[Khachhang] B on A.KhachhangID = B.KhachhangID WHERE A.ThangHoaDon=@thang AND A.NamHoaDon =@nam AND B.TuyenKHID=@tuyen";
                command.Parameters.AddWithValue("@thang",month);
                command.Parameters.AddWithValue("@nam",year);
                command.Parameters.AddWithValue("@tuyen",tuyenID);
                command.ExecuteNonQuery();
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
                command.CommandText = "UPDATE A SET [Trangthaicapnhathuy] = 1 FROM [dbo].[Hoadonnuocbihuy] A "+
                "JOIN [dbo].[Hoadonnuoc] B on A.HoadonnuocID = B.HoadonnuocID "+
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
            List<HoaDonNuocHaDong.Models.SoLieuTieuThu.HoaDonNuoc> danhSachHoaDon = filterChiSo(month,year,tuyenKHID);
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
    }
}