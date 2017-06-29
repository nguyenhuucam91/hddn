using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class MTuyenong:ModelBase
    {
        public int TOID { get { return GetINT(0); } set { SetINT(0, value); } }
        public string Ten { get { return GetSTR(1); } set { SetSTR(1, value); } }

        public string Captuyen { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public MTuyenong()
        {
            MaxPosModelField = 2;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}