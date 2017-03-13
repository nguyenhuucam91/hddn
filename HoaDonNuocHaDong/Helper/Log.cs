using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class Log
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        /// <summary>
        /// Lấy tên từ chức năng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static String getName(int id)
        {
            Nhomchucnang _nhom = db.Nhomchucnangs.FirstOrDefault(p => p.NhomchucnangID == id);
            if (_nhom != null)
            {
                return _nhom.Ten;
            }
            return "";
        }

        /// <summary>
        /// Lấy tên friendly từ controllerID
        /// </summary>
        /// <param name="id">controllerID</param>
        /// <returns></returns>
        public static String getControllerFriendlyName(int id)
        {
            Nhomchucnang _nhom = db.Nhomchucnangs.FirstOrDefault(p => p.NhomchucnangID == id);
            if (_nhom != null)
            {
                return _nhom.Ten;
            }
            return "";
        }

        /// <summary>
        /// Lấy tên friendly từ controllerID
        /// </summary>
        /// <param name="id">chuc năng chương trình ID</param>
        /// <returns></returns>
        public static String getActionFriendlyName(int id)
        {
            Chucnangchuongtrinh _cN = db.Chucnangchuongtrinhs.FirstOrDefault(p => p.ChucnangID == id);
            if (_cN != null)
            {
                return _cN.Ten;
            }
            return "";
        }

    }
}