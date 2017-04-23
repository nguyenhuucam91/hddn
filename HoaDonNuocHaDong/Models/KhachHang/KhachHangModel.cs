using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.KhachHang
{
    public class KhachHangModel : ModelBase
    {
        public int SoTT { get { return GetINT(0); } set { SetINT(0, value); } }
        public String MaKH { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public String maHopDong { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String tenKhachHang { get { return GetSTR(3); } set { SetSTR(3, value); } }      
        public String SoDT { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public int TTDoc { get { return GetINT(5); } set { SetINT(5, value); } }
        public int KHID { get { return GetINT(6); } set { SetINT(6, value); } }
        public String diaChi { get { return GetSTR(7); } set { SetSTR(7, value); } }
        public int Tinhtrang { get { return GetINT(8); } set { SetINT(8, value); } }
        public DateTime Ngayngungcapnuoc { get { return GetDT(9); } set { SetDT(9, value); } }
        public DateTime Ngaycapnuoclai { get { return GetDT(10); } set { SetDT(10, value); } }

        public int QuanhuyenID { get; set; }
        public int PhuongxaID { get; set; }
        public int CumdancuID { get; set; }
        public int TuyenKHID { get; set; }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}