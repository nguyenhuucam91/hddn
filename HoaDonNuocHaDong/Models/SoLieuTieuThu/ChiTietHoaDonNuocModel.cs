using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class ChiTietHoaDonNuocModel : ModelBase
    {
        public int ChiSoMoi { get { return GetINT(0); } set { SetINT(0, value); } }
        public int KhachHangId { get { return GetINT(1); } set { SetINT(1, value); } }

        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}