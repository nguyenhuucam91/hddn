using HoaDonNuocHaDong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class KiemDinh
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        /// <summary>
        /// xem danh sách kiểm định
        /// </summary>
        /// <param name="month">Tháng kiểm định</param>
        /// <param name="year">Năm kiểm định </param>
        /// <returns></returns>
        public IEnumerable<object> getDanhSachKiemDinh(int month, int year, int? tuyenID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            if (tuyenID == 0)
            {
                var dsKiemDinh = (from i in _db.Kiemdinhs
                                  join r in _db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                  where i.Ngaykiemdinh.Value.Month == month && i.Ngaykiemdinh.Value.Year == year
                                  select new
                                  {
                                      KhachHangID = r.KhachhangID,
                                      MaKhachHang = r.MaKhachHang,
                                      TenKhachHang = r.Ten,
                                      NgayKiemDinh = i.Ngaykiemdinh,
                                      ChiSoLucKiemDinh = i.Chisoluckiemdinh,
                                      KiemDinhID = i.KiemdinhID,
                                      GhiChu = i.Ghichu,
                                      ChiSoSauKiemDinh = i.Chisosaukiemdinh
                                  });
                return dsKiemDinh.ToList();
            }
            var dsKiemDinhTuyen = (from i in _db.Kiemdinhs
                              join r in _db.Khachhangs on i.KhachhangID equals r.KhachhangID
                              where i.Ngaykiemdinh.Value.Month == month && i.Ngaykiemdinh.Value.Year == year && r.TuyenKHID == tuyenID
                              select new
                              {
                                  KhachHangID = r.KhachhangID,
                                  MaKhachHang = r.MaKhachHang,
                                  TenKhachHang = r.Ten,
                                  NgayKiemDinh = i.Ngaykiemdinh,
                                  ChiSoLucKiemDinh = i.Chisoluckiemdinh,
                                  KiemDinhID = i.KiemdinhID,
                                  GhiChu = i.Ghichu,
                                  ChiSoSauKiemDinh = i.Chisosaukiemdinh
                              });
            return dsKiemDinhTuyen.ToList();
        }

        /// <summary>
        /// kiểm tra xem trong hóa đơn khách hàng đã kiểm định hay chưa
        /// </summary>
        /// <param name="khachHangID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool checkKiemDinhStatus(int khachHangID, int month, int year)
        {
            var countKiemDinh = db.Kiemdinhs.Count(p => p.KhachhangID == khachHangID && p.Ngaykiemdinh.Value.Month == month && p.Ngaykiemdinh.Value.Year == year);
            //nếu k có thì return false;
            if (countKiemDinh > 0)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Lấy chỉ số kiểm định của khách hàng ID trong tháng và năm
        /// </summary>
        /// <param name="KHID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int getChiSoLucKiemDinh(int KHID, int month, int year)
        {
            //lấy chỉ số kiểm đinh của khách hàng ID của tháng và năm
            var kiemDinh = db.Kiemdinhs.Where(p => p.KhachhangID == KHID && p.Ngaykiemdinh.Value.Month == month && p.Ngaykiemdinh.Value.Year == year);
            if (kiemDinh != null)
            {
                return kiemDinh.FirstOrDefault().Chisoluckiemdinh.Value;
            }
            return 0;
        }

        /// <summary>
        /// Lấy chỉ số sau khi kiểm định
        /// </summary>
        /// <param name="KHID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int getChiSoSauKiemDinh(int KHID, int month, int year)
        {
            //lấy chỉ số kiểm đinh của khách hàng ID của tháng và năm
            var kiemDinh = db.Kiemdinhs.Where(p => p.KhachhangID == KHID && p.Ngaykiemdinh.Value.Month == month && p.Ngaykiemdinh.Value.Year == year);
            if (kiemDinh != null)
            {
                return kiemDinh.FirstOrDefault().Chisosaukiemdinh.Value;
            }
            return 0;
        }
    }
}