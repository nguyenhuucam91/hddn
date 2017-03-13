using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HvitFramework;
namespace HoaDonNuocHaDong.ModelReport
{
    public class Apgia:ModelBase
    {
        public int ApgiaID { get { return GetINT(0); } set { SetINT(0,value); } }
        public int LoaiapgiaID { get { return GetINT(1); } set { SetINT(1, value); } }
        public Apgia()
        {
            MaxPosModelField = 1;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }

    }
}