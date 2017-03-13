using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaDonNuocHaDong.Repositories.Interfaces
{
    interface ITuyenKHRepository : IRepository<HDNHD.Models.DataContexts.Tuyenkhachhang>
    {
        IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> GetByNhanVienID(int nhanVienID);
    }
}
