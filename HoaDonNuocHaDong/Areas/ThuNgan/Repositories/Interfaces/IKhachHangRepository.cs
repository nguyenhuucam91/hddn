using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IKhachHangRepository : IRepository<HDNHD.Models.DataContexts.Khachhang>
    {
        IQueryable<KhachHangModel> GetAllKhachHangModel();
        KhachHangDetailsModel GetKhachHangDetailsModel(int id);
    }
}
