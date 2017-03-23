using HDNHD.Core.Helpers;
using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Repositories.Interfaces;
using HvitFramework.CoreData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class DangNhapRepository : LinqRepository<HDNHD.Models.DataContexts.Dangnhap>, IDangNhapRepository
    {
        public DangNhapRepository(AdminDataContext context) : base(context) { }

        public HDNHD.Models.DataContexts.Dangnhap CheckLogin(string loginToken, TimeSpan validSpan)
        {
            var validTime = DateTime.Now.Subtract(validSpan);
            var dangNhap = GetSingle(m => m.Trangthaikhoa != true
                && AuthHelpers.MD5(m.NguoidungID.ToString()) == loginToken
                && m.Thoigiandangnhap > validTime);
            return dangNhap;
        }


        public HDNHD.Models.DataContexts.Dangnhap GetByNguoiDungID(int nguoiDungID)
        {
            return GetSingle(m => m.NguoidungID == nguoiDungID);
        }
    }
}