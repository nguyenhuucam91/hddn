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
    
    public partial class Lichsusudungct
    {
        public int LichsuID { get; set; }
        public Nullable<int> ChucnangID { get; set; }
        public Nullable<int> NguoidungID { get; set; }
        public string Thaotac { get; set; }
        public Nullable<System.DateTime> Thoigian { get; set; }
    
        public virtual Chucnangchuongtrinh Chucnangchuongtrinh { get; set; }
        public virtual Nguoidung Nguoidung { get; set; }
    }
}
