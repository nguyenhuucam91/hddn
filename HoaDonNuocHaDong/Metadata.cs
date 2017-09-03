using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong
{
    public class KhachHangMetadata
    {
        public int KhachhangID { get; set; }
        public Nullable<int> QuanhuyenID { get; set; }
        public Nullable<int> PhuongxaID { get; set; }
        public Nullable<int> CumdancuID { get; set; }
        public Nullable<int> TuyenKHID { get; set; }
        public Nullable<int> LoaiKHID { get; set; }
        public Nullable<int> LoaiapgiaID { get; set; }
        public Nullable<int> HinhthucttID { get; set; }
        public string Sotaikhoan { get; set; }
        public string Masothue { get; set; }
        [Required(ErrorMessage = "Ngày kí hợp đồng không được để trống")]
        public Nullable<System.DateTime> Ngaykyhopdong { get; set; }
        [Required(ErrorMessage = "Tỉ lệ phí môi trường không để trống")]
        public Nullable<int> Tilephimoitruong { get; set; }
        [Required(ErrorMessage = "Số hộ không được để trống")]
        public Nullable<int> Soho { get; set; }
        public Nullable<System.DateTime> Ngayap { get; set; }
        public Nullable<System.DateTime> Ngayhetap { get; set; }
        [Required(ErrorMessage = "Số nhân khẩu không được để trống")]
        public Nullable<int> Sonhankhau { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống ")]
        public string Diachi { get; set; }

        public string Dienthoai { get; set; }
        public string Ghichu { get; set; }
        public Nullable<int> Sokhuvuc { get; set; }
        public string Sohopdong { get; set; }

        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        public string MaKhachHang { get; set; }
        public Nullable<int> Tinhtrang { get; set; }

        public string Diachithutien { get; set; }
        [Required(ErrorMessage = "Thứ tự đọc không để trống")]
        public Nullable<int> TTDoc { get; set; }

        public virtual Quanhuyen QuanHuyen { get; set; }

        //public virtual ICollection<Chitietchisonuoctheotuyen> Chitietchisonuoctheotuyens { get; set; }
        public virtual ICollection<Congno> Congnoes { get; set; }       
        public virtual Phuongxa Phuongxa { get; set; }
        public virtual Tuyenkhachhang Tuyenkhachhang { get; set; }
        public virtual ICollection<Hoadonnuoc> Hoadonnuocs { get; set; }

    }//end KhachHangMetadata

    public class KiemDinhMetadata
    {
        public int KiemdinhID { get; set; }
        public Nullable<int> KhachhangID { get; set; }
        [Required(ErrorMessage = "Ngày kiểm định không được để trống")]
        public Nullable<System.DateTime> Ngaykiemdinh { get; set; }
        public string Ghichu { get; set; }
        public Nullable<int> Chisoluckiemdinh { get; set; }
        public Nullable<int> Chisosaukiemdinh { get; set; }

    }// end class KiemDinhMetadata

    public class QuanHuyenMetadata
    {

        public int QuanhuyenID { get; set; }

        [Required(ErrorMessage = "Tên quận huyện không được để trống")]
        public string Ten { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phuongxa> Phuongxas { get; set; }
    }

    public class PhuongxaMetadata
    {
        public int PhuongxaID { get; set; }

        [Required(ErrorMessage = "Quận huyện không để trống")]
        public Nullable<int> QuanhuyenID { get; set; }

        [Required(ErrorMessage = "Tên phường xã không để trống")]
        [MaxLength(30, ErrorMessage = "Tên không dài quá 30 kí tự")]
        public string Ten { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cumdancu> Cumdancus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Khachhang> Khachhangs { get; set; }
        public virtual Quanhuyen Quanhuyen { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<To> Toes { get; set; }
    }


    public class CumdancuMetadata
    {
        public int CumdancuID { get; set; }
        [Required(ErrorMessage = "Phường xã không để trống")]
        public Nullable<int> PhuongxaID { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(30, ErrorMessage = "Tên không dài quá 30 kí tự")]
        public string Ten { get; set; }
        public Nullable<bool> IsDelete { get; set; }

    }

    public class ToQuanHuyenMetadata
    {
        public int ToQuanHuyenID { get; set; }

        [Required(ErrorMessage = "Mã không được để trống")]
        public string Ma { get; set; }
        public Nullable<int> SoCN { get; set; }
        public Nullable<int> QuanHuyenID { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }

    public class ApGiaMetadata
    {
        public int ApgiaID { get; set; }
        public Nullable<int> LoaiapgiaID { get; set; }
        [Range(1, 100, ErrorMessage = "Đến mức không âm")]
        public Nullable<int> Denmuc { get; set; }
        public Nullable<double> Gia { get; set; }
        public string Ten { get; set; }

        public Loaiapgia Loaiapgia { get; set; }
    }


    public class TuyenMetadata
    {

        public int TuyenKHID { get; set; }
        public Nullable<int> ToID { get; set; }
        public Nullable<int> CumdancuID { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Ten { get; set; }
        public string Diachi { get; set; }
        [Required(ErrorMessage = "Mã tuyến không được để trống")]
        public string Matuyen { get; set; }

    }

    public class ThongBaoMetadata
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Tieude { get; set; }
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Noidung { get; set; }
        public Nullable<int> Nguoitao { get; set; }
        public Nullable<System.DateTime> Ngaytao { get; set; }
        public Nullable<int> Nguoichinhsua { get; set; }
        public Nullable<System.DateTime> Ngaychinhsua { get; set; }
    }

    public class PhongbanMetata
    {
        public int PhongbanID { get; set; }
        public Nullable<int> ChinhanhID { get; set; }
        [Required(ErrorMessage = "Tên phòng ban không để trống")]
        public string Ten { get; set; }
    }

    public class NhanvienMetadata
    {
        public int NhanvienID { get; set; }

        public Nullable<int> PhongbanID { get; set; }

        public Nullable<int> ChucvuID { get; set; }
        [Required(ErrorMessage = "Tên nhân viên không để trống")]
        public string Ten { get; set; }
        public string SDT { get; set; }
        public string Diachi { get; set; }

        public Nullable<int> ToQuanHuyenID { get; set; }

        public virtual Chucvu Chucvu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Congno> Congnoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hoadonnuoc> Hoadonnuocs { get; set; }
        public virtual Phongban Phongban { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tuyentheonhanvien> Tuyentheonhanviens { get; set; }
    }

    /// <summary>
    /// Người dùng 
    /// </summary>
    public class NguoidungMetadata
    {
        public int NguoidungID { get; set; }
        public Nullable<int> NhanvienID { get; set; }
        [Required(ErrorMessage = "Tài khoản không để trống")]
        public string Taikhoan { get; set; }
        [Required(ErrorMessage = "Mật khẩu không để trống")]
        public string Matkhau { get; set; }
        public Nullable<bool> Isadmin { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dangnhap> Dangnhaps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lichsusudungct> Lichsusudungcts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quyencuanguoidung> Quyencuanguoidungs { get; set; }
    }

    public class TuyenongMetadata
    {
        public int TuyenongID { get; set; }
        public Nullable<int> TuyenongPID { get; set; }

        public Nullable<int> CaptuyenID { get; set; }
        [Required(ErrorMessage = "Mã tuyến không được để trống")]
        public String Matuyen { get; set; }
        [Required(ErrorMessage = "Tên tuyến không được để trống")]
        public string Tentuyen { get; set; }
    }
}