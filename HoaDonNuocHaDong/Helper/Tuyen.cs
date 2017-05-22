using HoaDonNuocHaDong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class Tuyen
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        HoaDonHaDongEntities _db = new HoaDonHaDongEntities();

        /// <summary>
        /// Lấy tên nhân viên theo tuyến ID
        /// </summary>
        /// <returns></returns>
        public String getTuyenCuaNhanVien(int tuyenID)
        {
            var tuyenTheoNhanVienList = (from p in _db.Tuyentheonhanviens
                                         join q in _db.Tuyenkhachhangs on p.TuyenKHID equals q.TuyenKHID
                                         join r in _db.Nhanviens on p.NhanVienID equals r.NhanvienID
                                         where p.TuyenKHID == tuyenID
                                         select new
                                         {
                                             TenNhanVien = r.Ten,
                                         }).FirstOrDefault();
            if (tuyenTheoNhanVienList != null)
            {
                return tuyenTheoNhanVienList.TenNhanVien;
            }
            return "";
        }

        /// <summary>
        /// Lấy tuyến của nhân viên quản lí dựa theo nhân viên ID
        /// </summary>
        /// <param name="nhanVienID"></param>
        /// <returns></returns>
        public static String getTuyenByNhanVienID(int nhanVienID)
        {
            var tuyenTheoNhanVienList = (from p in db.Tuyentheonhanviens
                                         join q in db.Tuyenkhachhangs on p.TuyenKHID equals q.TuyenKHID
                                         join r in db.Nhanviens on p.NhanVienID equals r.NhanvienID
                                         where p.NhanVienID == nhanVienID
                                         select new
                                         {
                                             TenTuyen = q.Ten,
                                         }).FirstOrDefault();
            if (tuyenTheoNhanVienList != null)
            {
                return tuyenTheoNhanVienList.TenTuyen;
            }
            return "";
        }

        public IQueryable<Tuyenkhachhang> getDanhSachTuyenByNhanVien(int nhanVienID)
        {
            var tuyenTheoNhanVien = from i in _db.Tuyentheonhanviens
                                    join r in _db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                                    join s in _db.Nhanviens on i.NhanVienID equals s.NhanvienID
                                    where i.NhanVienID == nhanVienID
                                    select r;
            return tuyenTheoNhanVien;
        }

        public int getNhanVienIDTuTuyen(int tuyenID)
        {
            var tuyenTheoNhanVienList = (from p in db.Tuyentheonhanviens
                                         join q in db.Tuyenkhachhangs on p.TuyenKHID equals q.TuyenKHID
                                         join r in db.Nhanviens on p.NhanVienID equals r.NhanvienID
                                         where p.TuyenKHID == tuyenID
                                         select new
                                         {
                                             IDNhanvien = r.NhanvienID,
                                         }).FirstOrDefault();
            if (tuyenTheoNhanVienList != null)
            {
                return tuyenTheoNhanVienList.IDNhanvien;
            }
            return 0;
        }

        /// <summary>
        /// Lấy danh sách tuyến theo tổ
        /// </summary>
        /// <param name="ToID"></param>
        /// <returns></returns>
        public List<Models.TuyenKhachHang.TuyenKhachHang> getTuyenByTo(int ToID)
        {
          
            var dsTuyen = (from i in _db.Tuyentheonhanviens
                           join r in _db.Nhanviens on i.NhanVienID equals r.NhanvienID
                           join t in _db.Tuyenkhachhangs on i.TuyenKHID equals t.TuyenKHID
                           join p in _db.ToQuanHuyens on r.ToQuanHuyenID equals p.ToQuanHuyenID
                           where (t.IsDelete == false || t.IsDelete == null) && r.ToQuanHuyenID == ToID
                           select new Models.TuyenKhachHang.TuyenKhachHang
                           {
                               TuyenCuaKH = t.TuyenKHID,
                               MaTuyenKH = t.Matuyen,
                               TenTuyen = t.Ten
                           }
                               ).Distinct().ToList();
            return dsTuyen;
        }

        public string getMaTuyenById(int TuyenKHID)
        {            
            Tuyenkhachhang tuyen = _db.Tuyenkhachhangs.Find(TuyenKHID);
            if (tuyen != null)
            {
                return tuyen.Matuyen;
            }
            return "";
        }
    }
}