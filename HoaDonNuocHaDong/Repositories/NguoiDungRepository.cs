using HDNHD.Core.Helpers;
using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class NguoiDungRepository : LinqRepository<HDNHD.Models.DataContexts.Nguoidung>, INguoiDungRepository
    {
        public NguoiDungRepository(AdminDataContext context) : base(context) { }
    }
}