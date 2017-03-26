using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class NhapChiSoCap:ModelBase
    {
        public int ChinhanhID { get { return GetINT(0); } set { SetINT(0, value); } }
        public string Ten { get { return GetSTR(1); } set { SetSTR(1, value); } }

     
        public NhapChiSoCap()
        {
            MaxPosModelField = 1;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}