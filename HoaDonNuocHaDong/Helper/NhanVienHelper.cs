using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class NhanVienHelper
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        String connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
        public String getPhongBan(int? nhanVienID)
        {
            HoaDonHaDongEntities db1 = new HoaDonHaDongEntities();
            if (nhanVienID != null)
            {
                var nhanVienObject = db1.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                if (nhanVienObject != null)
                {
                    var phongBanObject = db1.Phongbans.FirstOrDefault(p => p.PhongbanID == nhanVienObject.PhongbanID);
                    if (phongBanObject != null)
                    {
                        return phongBanObject.Ten;
                    }
                    return "";
                }
            }
            return "";
        }

        public static String getToQuanHuyen(int? nhanVienID)
        {
            if (nhanVienID != null)
            {
                var nhanVienObject = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                if (nhanVienObject != null)
                {
                    var toQuanHuyenObject = db.ToQuanHuyens.FirstOrDefault(p => p.ToQuanHuyenID == nhanVienObject.ToQuanHuyenID);
                    return toQuanHuyenObject.Ma;
                }
            }
            return "Không có";
        }

        public static String getChucVu(int? nhanVienID)
        {
            if (nhanVienID != null)
            {
                var nhanVienObject = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                if (nhanVienObject != null)
                {
                    var chucVuObject = db.Chucvus.FirstOrDefault(p => p.ChucvuID == nhanVienObject.ChucvuID);
                    return chucVuObject.Ten;
                }
            }
            return "Không có";
        }

        public static String getQuanHuyen(int? nhanVienID)
        {
            if (nhanVienID != null)
            {
                var nhanVienObject = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
                if (nhanVienObject != null)
                {
                    var quanHuyen = (from i in db.Quanhuyens
                                     join r in db.ToQuanHuyens on i.QuanhuyenID equals r.QuanHuyenID
                                     join s in db.Nhanviens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                                     where s.NhanvienID == nhanVienID
                                     select new
                                     {
                                         Ten = i.Ten
                                     }).FirstOrDefault();
                    return quanHuyen != null ? quanHuyen.Ten : "Không có";
                }
            }
            return "Không có";
        }

        public List<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHang> loadTuyenChuaCoNhanVien(int phongBanId)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            //List<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHang> tuyensKhachHang = new List<Models.TuyenKhachHang.TuyenKhachHang>();
            //if (phongBanId != 0)
            //{
            //    List<Nhanvien> nhanViens = _db.Nhanviens.Where(p => p.PhongbanID == phongBanId).ToList();
            //    var tuyens = (from i in _db.Tuyenkhachhangs
            //                  join j in _db.Tuyentheonhanviens on i.TuyenKHID equals j.TuyenKHID into j1
            //                  from j4 in j1.DefaultIfEmpty()
            //                  join r in _db.Nhanviens on j4.NhanVienID equals r.NhanvienID into j2
            //                  from j3 in j2.DefaultIfEmpty()
            //                  where (j4.TuyenKHID == null || j3.PhongbanID != phongBanId) && i.IsDelete == false
            //                  select new HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHang
            //                  {
            //                      MaTuyenKH = i.Matuyen,
            //                      TenTuyen = i.Ten,
            //                      TuyenKHID = i.TuyenKHID.ToString(),                                 
            //                  });
            //    return tuyens.ToList();
            //    //foreach (var tuyen in tuyens)
            //    //{
            //    //    if (nhanViens.Select(p => p.NhanvienID).Contains(tuyen.NhanVienId))
            //    //    {
            //    //        tuyensKhachHang.Add(tuyen);
            //    //    }
            //    //}

            //}
            //return tuyensKhachHang;

            var tuyens = (from i in _db.Tuyenkhachhangs
                          select new HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHang
                          {
                              MaTuyenKH = i.Matuyen,
                              TenTuyen = i.Ten,
                              TuyenKHID = i.TuyenKHID.ToString(),
                          }
                         ).ToList();
            return tuyens;
        }
    }
}