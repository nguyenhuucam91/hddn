using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHang : ModelBase
    {      
        public String MaKH { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public String maHopDong { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String tenKhachHang { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public String diaChi { get { return GetSTR(4); } set { SetSTR(4, value); } }      
        public String SoDT { get { return GetSTR(5); } set { SetSTR(5, value); } }        
        public int TTDoc { get { return GetINT(6); } set { SetINT(6, value); } }                     
        
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}