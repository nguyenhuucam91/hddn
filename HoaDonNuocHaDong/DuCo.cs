//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HoaDonNuocHaDong
{
    using System;
    using System.Collections.Generic;
    
    public partial class DuCo
    {
        public int DuCoID { get; set; }
        public Nullable<int> TienNopTheoThangID { get; set; }
        public Nullable<int> KhachhangID { get; set; }
        public Nullable<int> SoTienDu { get; set; }
        public bool TrangThaiTruHet { get; set; }
        public Nullable<System.DateTime> NgayTruHet { get; set; }
    
        public virtual Khachhang Khachhang { get; set; }
        public virtual SoTienNopTheoThang SoTienNopTheoThang { get; set; }
    }
}
