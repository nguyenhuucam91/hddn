using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Helper
{
    public class NguoidungHelper
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        BaseController baseCtl = new BaseController();
        /// <summary>
        /// Lấy tên nhân viên từ nhân viên ID
        /// </summary>
        /// <param name="nhanVienID"></param>
        /// <returns></returns>
        public static String getTenNhanVien(int nhanVienID)
        {
            var nVien = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
            //nếu tìm thấy nhân viên trong hệ thống
            if (nVien != null)
            {
                return nVien.Ten;
            }
            return "";
        }
        /// <summary>
        /// Lấy tên đăng nhập của người dùng dựa trên nhân viên ID
        /// </summary>
        /// <param name="nhanVienID"></param>
        /// <returns></returns>
        public static String getTenDangNhap(int nguoiDungID)
        {
            Nguoidung nguoiDung = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
            //nếu người dùng !=null
            if (nguoiDung != null)
            {
                return nguoiDung.Taikhoan;
            }
            return "";
        }

        /// <summary>
        /// Kiểm tra xem tình trạng tài khoản có bị khóa hay không
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public String getTrangThaiKhoa(int nguoiDungID)
        {

            Dangnhap dangNhap = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == nguoiDungID);

            if (dangNhap != null)
            {
                return dangNhap.Trangthaikhoa.Value ? "Khóa" : "";
            }
            return "Không có";
        }

        /// <summary>
        /// Lấy số lần đăng nhập sai của người dùng
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public static int getSoLanDangNhapSai(int nguoiDungID)
        {
            Dangnhap dangNhap = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
            if (dangNhap != null)
            {
                return dangNhap.Solandangnhapsai.Value;
            }
            return -1;
        }

        /// <summary>
        /// Lấy thời gian hết hạn khóa của người dùng
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public static DateTime? getThoiGianHetHan(int nguoiDungID)
        {
            Dangnhap dangNhap = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
            if (dangNhap != null)
            {
                return dangNhap.Thoigianhethankhoa;
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Lấy thời gian đăng nhập gần nhất của người dùng
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public static String getThoiGianDangNhap(int nguoiDungID)
        {
            Dangnhap dangNhap = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == nguoiDungID);

            if (dangNhap != null)
            {
                DateTime? timeLogin = dangNhap.Thoigiandangnhap;
                if (timeLogin != null)
                {
                    return timeLogin.Value.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    return "";
                }
            }
            return "";
        }

        /// <summary>
        /// Hàm để lấy quận huyện của người dùng
        /// </summary>
        /// <param name="nguoiDungID">ID người dùng</param>
        /// <param name="option">Nếu option = 0 thì lấy quận huyện ID, nếu option = 1 thì lấy tên chi nhánh</param>
        /// <returns></returns>
        public static object getChiNhanhCuaNguoiDung(int nguoiDungID, int option)
        {
            var chiNhanh = (from i in db.Nguoidungs
                            join r in db.Nhanviens on i.NhanvienID equals r.NhanvienID
                            join s in db.ToQuanHuyens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                            join p in db.Quanhuyens on s.QuanHuyenID equals p.QuanhuyenID
                            where i.NguoidungID == nguoiDungID
                            select new
                            {
                                ChiNhanhID = p.QuanhuyenID,
                                TenChiNhanh = p.Ten,
                            }).FirstOrDefault();
            if (chiNhanh != null)
            {
                if (option == 0)
                {
                    return chiNhanh.ChiNhanhID;
                }
                else if (option == 1)
                {
                    return chiNhanh.TenChiNhanh;
                }
            }
            return 0;
        }

        /// <summary>
        /// Lấy tổ của người dùng thuộc phòng ban ID và quận huyện ID
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public List<HoaDonNuocHaDong.Models.ToQuanHuyen.ToQuanHuyen> getPhongBanCuaNguoiDung(int phongbanID, int quanHuyenID, int option)
        {

            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            var toQH = (from i in _db.Nguoidungs
                        join r in _db.Nhanviens on i.NhanvienID equals r.NhanvienID
                        join s in _db.ToQuanHuyens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                        where s.PhongbanID == phongbanID && s.ToQuanHuyenID == quanHuyenID
                        select new HoaDonNuocHaDong.Models.ToQuanHuyen.ToQuanHuyen
                        {
                            ToQuanHuyenID = s.ToQuanHuyenID,
                            Ten = s.Ma
                        });
            return toQH.ToList();
        }

       
        /// <summary>
        /// Lấy nhân viên ID từ người dùng ID
        /// </summary>
        /// <param name="ngDungID"></param>
        /// <returns></returns>

        public int getNhanVienIDFromNguoiDungID(int ngDungID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            Nguoidung ngDungObj = _db.Nguoidungs.Find(ngDungID);
            if (ngDungObj != null)
            {
                return ngDungObj.NhanvienID.Value;
            }
            return 0;
        }


        public bool isNguoiDungLaTruongPhong(int? nhanVienId)
        {
            bool isOnlyTruongPhong = false;
            if (nhanVienId != null)
            {
                int userRole = baseCtl.getUserRole(nhanVienId);
                if (userRole == (int)EChucVu.TRUONG_PHONG)
                {
                    isOnlyTruongPhong = true;
                }
            }
            return isOnlyTruongPhong;
        }


        
    }
}