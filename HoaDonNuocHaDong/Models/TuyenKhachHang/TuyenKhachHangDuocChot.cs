using HoaDonNuocHaDong.Config;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.TuyenKhachHang
{
    public class TuyenKhachHangDuocChot : ModelBase
    {        
        public int TuyenKHID { get { return GetINT(0); } set {  SetINT(0, value); } }
        public int TrangThaiTinhTien { get; set; }
        public int soLuongKhachHangDaIn { get { return GetINT(1); } set { SetINT(1, value); } }
        public int tongSoLuongKhachHang { get { return GetINT(2); } set { SetINT(2, value); } }
        public DateTime? ngayInXong { get { return GetDT(3); } set { SetDT(3, value); } }
        public double TongSoTienTuyen { get { return GetD(4); } set { SetD(4, value); } }
        public int TuyenCuaKH { get; set; }
        public string MaTuyenKH { get; set; }
        public string TenTuyen { get; set; }
        public int NhanVienId { get; set; }       

        public TuyenKhachHangDuocChot getThongTinTungTuyen(int tuyenKHID, int month, int year)
        {            
            TuyenKhachHangDuocChot tuyenKhachHangDuocChot = new TuyenKhachHangDuocChot();
            String connectionString = DatabaseConfig.getConnectionString();
            var conn = new SqlConnection(connectionString);
            conn.Open();
            var command = new SqlCommand("TongSoLuongKhachHangInHoaDon", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@thang", month);
            command.Parameters.AddWithValue("@nam", year);
            command.Parameters.AddWithValue("@tuyen", tuyenKHID);
            SqlDataReader reader = command.ExecuteReader();           
            while (reader.Read())
            {                             
                tuyenKhachHangDuocChot.soLuongKhachHangDaIn = reader.GetInt32(reader.GetOrdinal("TTIn"));
                tuyenKhachHangDuocChot.tongSoLuongKhachHang = reader.GetInt32(reader.GetOrdinal("TongSoKhachHang"));
                int colNgayInPosition = reader.GetOrdinal("NgayIn");
                if(!reader.IsDBNull(colNgayInPosition)){
                    tuyenKhachHangDuocChot.ngayInXong = reader.GetDateTime(reader.GetOrdinal("NgayIn"));
                }
                else
                {
                    tuyenKhachHangDuocChot.ngayInXong = null;
                }
                tuyenKhachHangDuocChot.TongSoTienTuyen = reader.GetDouble(reader.GetOrdinal("TongCong"));
            }
            conn.Close();
            return tuyenKhachHangDuocChot;

        }

        protected override Type TransferType()
        {
            return this.GetType();
        }

    }
}