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
    
    public partial class Thongbao
    {
        public int Id { get; set; }
        public string Tieude { get; set; }
        public string Noidung { get; set; }
        public Nullable<int> Nguoitao { get; set; }
        public Nullable<System.DateTime> Ngaytao { get; set; }
        public Nullable<int> Nguoichinhsua { get; set; }
        public Nullable<System.DateTime> Ngaychinhsua { get; set; }
    
        public virtual Nguoidung Nguoidung { get; set; }
    }
}
