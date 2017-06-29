using HoaDonNuocHaDong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{

    /// <summary>
    /// Class này phục vụ cho mục đích lấy cấu hình hiện tại của hệ thống,
    /// ví dụ tiền nước SH1, SH2, SH3,...
    /// </summary>
    public class ConfigHelper
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        /// <summary>
        /// Hàm để lấy số tiền đc quy hoạch trong CSDL, ví dụ như SH1 (0-10m3) có giá là 10000 đồng
        /// lấy số 10000 đồng đó
        /// </summary>
        /// <param name="key">Tên loại áp giá (SH1, SH2,...)</param>
        /// <returns></returns>
        public double getKeyFromConfig(String key)
        {
            var soTien = (from i in db.Khachhangs
                         join j in db.Loaiapgias on i.LoaiapgiaID equals j.LoaiapgiaID
                         join k in db.Apgias on i.LoaiapgiaID equals k.LoaiapgiaID
                         where j.Ten == key
                         select new
                         {
                             SoTien = k.Gia
                         }).FirstOrDefault();
            //nếu KEY không hợp lệ hoặc ko tìm thấy trong db, thì return -1, nếu có thì return số tiền
            if (soTien != null)
            {
                return soTien.SoTien.Value;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String getStringValue(String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return "";
        }


    }
}