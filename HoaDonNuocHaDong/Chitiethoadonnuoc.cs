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
    
    public partial class Chitiethoadonnuoc
    {
        public int ChitiethoadonnuocID { get; set; }
        public Nullable<int> HoadonnuocID { get; set; }
        public Nullable<int> Chisocu { get; set; }
        public Nullable<int> Chisomoi { get; set; }
        public Nullable<double> SH1 { get; set; }
        public Nullable<double> SH2 { get; set; }
        public Nullable<double> SH3 { get; set; }
        public Nullable<double> SH4 { get; set; }
        public Nullable<double> HC { get; set; }
        public Nullable<double> CC { get; set; }
        public Nullable<double> SXXD { get; set; }
        public Nullable<double> KDDV { get; set; }
    
        public virtual Hoadonnuoc Hoadonnuoc { get; set; }
    }
}
