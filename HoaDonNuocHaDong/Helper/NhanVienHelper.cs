using HoaDonNuocHaDong.Models;
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
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
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

        public String getToQuanHuyen(int? nhanVienID)
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

        public String getChucVu(int? nhanVienID)
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

        public int getQuanHuyen(int? nhanVienID)
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
                                     select new ModelQuanHuyen
                                     {
                                         QuanHuyenID = i.QuanhuyenID,
                                     }
                                     ).FirstOrDefault();
                    return quanHuyen.QuanHuyenID;
                }
            }
            return 0;
        }

        public List<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHang> loadTuyenChuaCoNhanVien()
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();

            var tuyens = (from i in _db.Tuyenkhachhangs
                          where i.IsDelete == false
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