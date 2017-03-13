using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DuNoFilterModel : BaseFilterModel<DuNoModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override void ApplyFilter(ref IQueryable<DuNoModel> items)
        {
            var context = (HDNHDDataContext)GetDataContext(items);

            if (Year != null)
            {
                items = items.Where(m => m.HoaDon.NamHoaDon == Year);
                if (Month != null)
                {
                    items = items.Where(m => m.HoaDon.ThangHoaDon == Month);
                }
            }

            // join all 

            if (NhanVienID != null)
            {
                items = from dnm in items
                        join nv in context.Nhanviens on dnm.HoaDon.NhanvienID equals NhanVienID
                        select new DuNoModel()
                        {
                            HoaDon = dnm.HoaDon,
                            NhanVien = nv,
                        };
            }
        }
    }
}