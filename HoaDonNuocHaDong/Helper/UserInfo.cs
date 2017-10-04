using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using HoaDonNuocHaDong;

namespace HoaDonNuocHaDong.Helper
{
    /// <summary>
    /// Trong class này sẽ lấy thông người người dùng đăng nhập
    /// bao gồm role, phòng ban của người dùng đăng nhập
    /// </summary>
    public class UserInfo
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public const int SOLANDANGNHAPSAI = 5;
        //số ngày hết hạn khóa
        public const int DATETHRESHOLD = 1;
       
        /// <summary>
        /// Lấy số lần đăng nhập sai của người dùng <code>nguoiDungID</code>
        /// </summary>
        /// <returns></returns>
        public static int getSoLanDangNhapSai(int nguoiDungID)
        {
            var dangNhap = db.Dangnhaps.Where(p => p.NguoidungID == nguoiDungID).FirstOrDefault();
            if (dangNhap != null)
            {
                return dangNhap.Solandangnhapsai.Value;
            }
            return -1;
        }


        /// <summary>
        /// Hàm tạo md5 của <code>input</code>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Lấy chức vụ của người dùng có ID = <code>nguoiDungID</code>
        /// </summary>
        /// <param name="nguoiDungID">ID người dùng</param>
        /// <returns>Role của người dùng</returns>
        public Phongban getPhongBan(int nguoiDungID)
        {
            int nhanVienID = getNhanVienID(nguoiDungID);
            //nếu có nhân viên ID từ người dùng thì lấy Phòng ban object ra
            if (nhanVienID != -1)
            {
                var roleRecord = db.Nhanviens.Where(p => p.NhanvienID == nhanVienID).FirstOrDefault();
                if (roleRecord != null)
                {
                    Phongban pB = roleRecord.Phongban;
                    if (pB != null)
                    {
                        return pB;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra xem người dùng là admin hay không
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public static bool checkAdmin(int nguoiDungID)
        {
            var ngDung = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == nguoiDungID);
            if (ngDung != null)
            {
                if (ngDung.Isadmin == null || ngDung.Isadmin == false)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// lấy thông tin đăng nhập từ người dùng ID
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public Dangnhap getDangNhap(int nguoiDungID)
        {
            var dangNhap = db.Dangnhaps.Where(p => p.NguoidungID == nguoiDungID).FirstOrDefault();
            if (dangNhap != null)
            {
                db.Entry(dangNhap).State = EntityState.Detached;
                return dangNhap;
            }                  
            return null;
        }

        /// <summary>
        /// Lấy nhân viên ID từ người dùng ID
        /// </summary>
        /// <param name="nguoiDungID"></param>
        /// <returns></returns>
        public static int getNhanVienID(int nguoiDungID)
        {
            var dangNhap = db.Nguoidungs.Where(p => p.NguoidungID == nguoiDungID).FirstOrDefault();
            if (dangNhap != null)
            {
                return dangNhap.NhanvienID.Value;
            }
            return -1;
        }

        
    }
}