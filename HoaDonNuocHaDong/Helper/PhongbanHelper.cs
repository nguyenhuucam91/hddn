using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class PhongbanHelper
    {
        public const int KINHDOANH = 1006;
        public const int THUNGAN = 1007;
        public const int INHOADON = 1008;
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        /// <summary>
        /// Đếm số nhân viên trong phòng ban
        /// </summary>
        /// <param name="phongBanID"></param>
        /// <returns></returns>
        public static int countNhanVienInPhongban(int phongBanID)
        {
            return db.Nhanviens.Count(p => p.PhongbanID == phongBanID);
        }

        /// <summary>
        /// Lấy danh sách nhân viên thuộc phòng ban
        /// </summary>
        /// <param name="phongbanID"></param>
        /// <returns></returns>
        public static List<Nhanvien> getNhanVienInPhongBan(int phongbanID)
        {
            List<Nhanvien> nhanVien = db.Nhanviens.Where(p => p.PhongbanID == phongbanID).OrderBy(p => p.Ten).ToList();
            return nhanVien;
        }

        /// <summary>
        /// Lấy tên phòng ban dựa theo phòng ban ID
        /// </summary>
        /// <param name="phongBanID"></param>
        /// <returns></returns>
        public string getTenPhongBan(int? phongBanID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            if (phongBanID != null)
            {
                return _db.Phongbans.Find(phongBanID).Ten;
            }
            return "";
        }
    }
}