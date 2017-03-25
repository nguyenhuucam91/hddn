using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangApGiaChung : ModelBase
    {
        public int STT { get; set; }
        public String MaKH { get; set; }
        public String HoTen { get; set; }
        public String DiaChi { get; set; }
        public String Tuyen { get; set; }
        public int? TTDoc { get; set; }
        public String CachTinh { get; set; }
        public int KhachHangID { get; set; }
        public String SinhHoat { get; set; }
        public String SanXuat { get; set; }
        public String HanhChinh { get; set; }
        public String CongCong { get; set; }
        public String KinhDoanh { get; set; }

        public String SH1 { get; set; }
        public String SH2 { get; set; }
        public String SH3 { get; set; }
        public String SH4 { get; set; }

        public DanhSachKhachHangApGiaChung()
        {
            MaxPosModelField = 6;
        }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}