using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.KhachHang
{
    public class LocChiSo :ModelBase
    {
        public int count { get { return GetINT(0); } set { SetINT(0, value); } }

        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}