using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaDonNuocHaDong.Repositories.Interfaces
{
    public interface IDangNhapRepository : IRepository<HDNHD.Models.DataContexts.Dangnhap>
    {
        HDNHD.Models.DataContexts.Dangnhap CheckLogin(string loginToken, TimeSpan validSpan);
        HDNHD.Models.DataContexts.Dangnhap GetByNguoiDungID(int nguoiDungID);
    }
}
