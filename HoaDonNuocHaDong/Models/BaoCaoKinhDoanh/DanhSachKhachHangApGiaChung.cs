using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangApGiaChung : ModelBase
    {

        public String MaKH { get { return GetSTR(0); } set { SetSTR(0, value); } }
        public String HoTen { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public String DiaChi { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String Tuyen { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public int? TTDoc { get { return GetINT(4); } set { SetINT(4, value); } }
        public String CachTinh { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public int KhachHangID { get { return GetINT(6); } set { SetINT(6, value); } }
        public String SinhHoat { get; set; }
        public String SanXuat { get; set; }
        public String HanhChinh { get; set; }
        public String CongCong { get; set; }
        public String KinhDoanh { get; set; }

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