using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaDonNuocHaDong.Repositories.Interfaces
{
    public interface INhanVienRepository : IRepository<HDNHD.Models.DataContexts.Nhanvien>
    {
        IQueryable<HDNHD.Models.DataContexts.Nhanvien> GetByToID(int toID);
    }
}
