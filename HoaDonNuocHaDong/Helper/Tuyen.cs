﻿using HoaDonNuocHaDong;
using HoaDonNuocHaDong.Config;
using HoaDonNuocHaDong.Models.TuyenKhachHang;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public IEnumerable<Tuyenkhachhang> getDanhSachTuyensByNhanVien(int? nhanVienId)
        {
            IEnumerable<Tuyenkhachhang> tuyensKhachHang = null;
            if (nhanVienId != 0)
            {
                var tuyenTheoNhanVien = (from p in _db.Tuyentheonhanviens
                                         join r in _db.Tuyenkhachhangs on p.TuyenKHID equals r.TuyenKHID
                                         join s in _db.Nhanviens on p.NhanVienID equals s.NhanvienID
                                         where p.NhanVienID == nhanVienId
                                         select new
                                         {
                                             TuyenKhachHang = r
                                         });
                tuyensKhachHang = tuyenTheoNhanVien.Select(p => p.TuyenKhachHang);
            }
            //nếu không sẽ tiến hành lọc tuyến theo chi nhánh
            if (nhanVienId == null || nhanVienId == 0)
            {
                tuyensKhachHang = _db.Tuyenkhachhangs.OrderByDescending(p => p.TuyenKHID);
            }
            return tuyensKhachHang;
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

        public String getTTTuyen(int TuyenKHID)
        {
            Tuyenkhachhang tuyen = _db.Tuyenkhachhangs.Find(TuyenKHID);
            if (tuyen != null)
            {
                return tuyen.Matuyen + " - " + tuyen.Ten;
            }
            return "";
        }

        public String translateTuyenIDToMaTuyen(String tuyenIdsInput)
        {
            String maTuyens = "";
            String[] maTuyensAsArray = tuyenIdsInput.Split(',');
            foreach (var item in maTuyensAsArray)
            {
                int tuyenId = Convert.ToInt32(item);
                Tuyenkhachhang tuyen = db.Tuyenkhachhangs.Find(tuyenId);
                if (tuyen != null)
                {
                    maTuyens += tuyen.Matuyen + ",";
                }
            }
            return maTuyens.Trim(',');
        }

        public IEnumerable<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> getDanhSachTuyensDuocChot(int? quanHuyen, int? to, int? selectedNhanVien, int? month, int? year)
        {
            List<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> tuyensKhachHangDuocChot = new List<TuyenKhachHangDuocChot>();
            if (quanHuyen == 0)
            {
                ControllerBase<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> cB = new ControllerBase<TuyenKhachHangDuocChot>();
                tuyensKhachHangDuocChot = cB.Query("DanhSachTuyenThuocToTheoThangNam",
                            new SqlParameter("@quanhuyen", 0),
                            new SqlParameter("@to", to),
                            new SqlParameter("@nhanvien", selectedNhanVien),
                            new SqlParameter("@month", month),
                            new SqlParameter("@year", year)).ToList();
            }

            else if (quanHuyen != 0 && to == null)
            {
               ControllerBase<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> cB = new ControllerBase<TuyenKhachHangDuocChot>();
                tuyensKhachHangDuocChot = cB.Query("DanhSachTuyenThuocToTheoThangNam",
                             new SqlParameter("@quanhuyen", quanHuyen),
                             new SqlParameter("@to", 0),
                             new SqlParameter("@nhanvien", selectedNhanVien),
                             new SqlParameter("@month", month),
                             new SqlParameter("@year", year)).ToList();
            }
            else if (quanHuyen != 0 && to != 0 && selectedNhanVien == null)
            {
                ControllerBase<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> cB = new ControllerBase<TuyenKhachHangDuocChot>();
                tuyensKhachHangDuocChot = cB.Query("DanhSachTuyenThuocToTheoThangNam",
                            new SqlParameter("@quanhuyen", quanHuyen),
                            new SqlParameter("@to", to),
                            new SqlParameter("@nhanvien", 0),
                            new SqlParameter("@month", month),
                            new SqlParameter("@year", year)).ToList();
            }
            else if (quanHuyen != 0 && to != 0 && selectedNhanVien != 0)
            {
                ControllerBase<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> cB = new ControllerBase<TuyenKhachHangDuocChot>();
                tuyensKhachHangDuocChot = cB.Query("DanhSachTuyenThuocToTheoThangNam",
                             new SqlParameter("@quanhuyen", quanHuyen),
                             new SqlParameter("@to", to),
                             new SqlParameter("@nhanvien", selectedNhanVien),
                             new SqlParameter("@month", month),
                             new SqlParameter("@year", year)).ToList();
            }

            return tuyensKhachHangDuocChot.AsEnumerable();
        }
    }
}