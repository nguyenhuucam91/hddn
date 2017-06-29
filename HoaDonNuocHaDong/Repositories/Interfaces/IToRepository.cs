using HDNHD.Core.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaDonNuocHaDong.Repositories.Interfaces
{
    interface IToRepository : IRepository<HDNHD.Models.DataContexts.ToQuanHuyen>
    {
        IQueryable<HDNHD.Models.DataContexts.ToQuanHuyen> GetByQuanHuyenID(int quanHuyenID);

        IQueryable<HDNHD.Models.DataContexts.ToQuanHuyen> Query(int quanHuyenID, int? phongBanID);
    }
}
