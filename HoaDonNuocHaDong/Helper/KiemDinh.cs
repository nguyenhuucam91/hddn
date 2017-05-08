using HoaDonNuocHaDong;
using HoaDonNuocHaDong.Models.KhachHang;
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
        public List<HoaDonNuocHaDong.Models.KhachHang.KiemDinhModel> getDanhSachKiemDinh(int? month, int? year, int? tuyenID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            if (tuyenID == 0)
            {
                var dsKiemDinh = (from i in _db.Kiemdinhs
                                  join t in _db.Hoadonnuocs on i.HoaDonId equals t.HoadonnuocID
                                  join s in _db.Chitiethoadonnuocs on i.HoaDonId equals s.HoadonnuocID
                                  join r in _db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                  where t.ThangHoaDon == month && t.NamHoaDon == year
                                  select new HoaDonNuocHaDong.Models.KhachHang.KiemDinhModel
                                  {
                                      KhachHangID = r.KhachhangID,
                                      MaKhachHang = r.MaKhachHang,
                                      TenKhachHang = r.Ten,
                                      NgayKiemDinh = i.Ngaykiemdinh.Value,
                                      ChiSoCu = s.Chisocu.Value,
                                      ChiSoLucKiemDinh = i.Chisoluckiemdinh.Value,
                                      KiemDinhID = i.KiemdinhID,
                                      GhiChu = i.Ghichu,
                                      ChiSoSauKiemDinh = i.Chisosaukiemdinh.Value
                                  });
                return dsKiemDinh.ToList();
            }
            var dsKiemDinhTuyen = (from i in _db.Kiemdinhs
                                   join t in _db.Hoadonnuocs on i.HoaDonId equals t.HoadonnuocID
                                   join s in _db.Chitiethoadonnuocs on i.HoaDonId equals s.HoadonnuocID
                                   join r in _db.Khachhangs on i.KhachhangID equals r.KhachhangID
                                   where t.ThangHoaDon == month && t.NamHoaDon == year && r.TuyenKHID == tuyenID
                                   select new HoaDonNuocHaDong.Models.KhachHang.KiemDinhModel
                              {
                                  KhachHangID = r.KhachhangID,
                                  MaKhachHang = r.MaKhachHang,
                                  TenKhachHang = r.Ten,
                                  NgayKiemDinh = i.Ngaykiemdinh.Value,
                                  ChiSoLucKiemDinh = i.Chisoluckiemdinh.Value,
                                  KiemDinhID = i.KiemdinhID,
                                  GhiChu = i.Ghichu,
                                  ChiSoSauKiemDinh = i.Chisosaukiemdinh.Value
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
        public bool checkKiemDinhStatus(int khachHangID, int month, int year)
        {
            var isKiemDinh = (from i in db.Kiemdinhs
                              join r in db.Hoadonnuocs on i.HoaDonId equals r.HoadonnuocID
                              where r.KhachhangID == khachHangID && r.ThangHoaDon == month && r.NamHoaDon == year
                              select new { 
                              
                              }).Count();
            //nếu k có thì return false;
            if (isKiemDinh > 0)
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
            var kiemDinh = (from i in db.Kiemdinhs
                            join r in db.Hoadonnuocs on i.HoaDonId equals r.HoadonnuocID
                            where r.ThangHoaDon == month && r.NamHoaDon == year
                            select new KiemDinhModel
                            {
                                ChiSoLucKiemDinh = i.Chisoluckiemdinh.Value,
                            }).FirstOrDefault();
            if (kiemDinh != null)
            {
                return kiemDinh.ChiSoLucKiemDinh;
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
            var kiemDinh = (from i in db.Kiemdinhs
                            join r in db.Hoadonnuocs on i.HoaDonId equals r.HoadonnuocID
                            where r.ThangHoaDon == month && r.NamHoaDon == year
                            select new KiemDinhModel
                            {
                                ChiSoSauKiemDinh = i.Chisosaukiemdinh.Value,
                            }).FirstOrDefault();
            if (kiemDinh != null)
            {
                return kiemDinh.ChiSoSauKiemDinh;
            }
            return 0;
        }

        public List<KiemDinhModel> getDanhSachKiemDinhCuaKhachHang(int khachHangID)
        {
            var kiemDinhs = (from i in db.Kiemdinhs
                             join r in db.Hoadonnuocs on i.HoaDonId equals r.HoadonnuocID
                             join s in db.Chitiethoadonnuocs on r.HoadonnuocID equals s.HoadonnuocID
                             where r.KhachhangID.Value == khachHangID
                             select new KiemDinhModel
                             {
                                 NgayKiemDinh = i.Ngaykiemdinh.Value,
                                 ChiSoCu = s.Chisocu.Value,
                                 ChiSoLucKiemDinh = i.Chisoluckiemdinh.Value,
                                 ChiSoSauKiemDinh = i.Chisosaukiemdinh.Value,
                                 ChiSoMoi = s.Chisomoi.Value,
                                 GhiChu = i.Ghichu
                             }).ToList();
            return kiemDinhs;
        }
    }
}