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
    
    public partial class Quanhuyen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quanhuyen()
        {
            this.Chinhanhs = new HashSet<Chinhanh>();
            this.Phuongxas = new HashSet<Phuongxa>();
        }
    
        public int QuanhuyenID { get; set; }
        public string Ten { get; set; }
        public string DienThoai { get; set; }
        public string DienThoai2 { get; set; }
        public string DienThoai3 { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chinhanh> Chinhanhs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phuongxa> Phuongxas { get; set; }
    }
}
