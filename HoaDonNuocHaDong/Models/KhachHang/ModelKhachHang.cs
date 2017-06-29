using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.KhachHang
{
    public class ModelKhachHang
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;

        private int soHo { get; set; }
        private DateTime ngayHetAp { get; set; }

        /// <summary>
        /// Cập nhật danh sách khách hàng hết hạn áp giá
        /// </summary>
        public void updateKHHetHanApGia(DateTime currDate)
        {            
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "Update Khachhang set Soho=@soHo WHERE Ngayhetap<@currDate";
                command.Parameters.AddWithValue("@soHo",1);
                command.Parameters.AddWithValue("@currDate", currDate);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}