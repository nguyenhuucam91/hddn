using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoInHoaDon
{
    public class BaoCaoPhiNuocThai:ModelBase
    {
        public int SoTT { get { return GetINT(0); } set { SetINT(0, value); } }       
        public String tenKhachHang { get { return GetSTR(1); } set { SetSTR(1, value); } }        
        public String diaChi { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String Tuyen { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public String MaKH { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public double TruocThue { get { return GetD(5); } set { SetD(5, value); } }
        public double Thue { get { return GetD(6); } set { SetD(6, value); } }
        public double Tong { get { return GetD(7); } set { SetD(7, value); } }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}