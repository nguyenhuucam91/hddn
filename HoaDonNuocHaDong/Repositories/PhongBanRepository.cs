using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class PhongBanRepository : LinqRepository<HDNHD.Models.DataContexts.Phongban>, IPhongBanRepository
    {
        public PhongBanRepository(HDNHDDataContext context) : base(context) { }
    }
}