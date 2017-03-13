using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HvitFramework;

namespace HoaDonNuocHaDong.Models
{
    public class ChisocapModel : ModelBase
    {
        public int ChisocapID { get { return GetINT(0); } set { SetINT(0, value); } }

        public int TuyenongID { get { return GetINT(1); } set { SetINT(1, value); } }

        public int Thang { get { return GetINT(2); } set { SetINT(2, value); } }

        public int Nam { get { return GetINT(3); } set { SetINT(3, value); } }

        public int Chiso { get { return GetINT(4); } set { SetINT(4, value); } }

        public ChisocapModel()
        {
            MaxPosModelField = 4;
        }

        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}