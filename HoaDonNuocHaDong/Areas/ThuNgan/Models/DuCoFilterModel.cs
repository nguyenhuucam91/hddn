using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DuCoFilterModel : BaseFilterModel<DuCoModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override void ApplyFilter(ref IQueryable<DuCoModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);
            
            // year
            if (Year != null)
            {
                items = items.Where(m => m.HoaDon.NamHoaDon == Year);
                // month
                if (Month != null)
                {
                    items = items.Where(m => m.HoaDon.ThangHoaDon == Month);
                }
            }

            // nhan vien
            if (NhanVienID != null)
            {
                items = items.Where(m => m.HoaDon.NhanvienID == NhanVienID);
            } // to
            else if (ToID != null)
            {
                items = from item in items
                        join to in context.ToQuanHuyens on item.KhachHang.QuanhuyenID equals to.QuanHuyenID
                        where to.ToQuanHuyenID == ToID
                        select item;
            } // quan huyen
            else if (QuanHuyenID != null)
            {
                items = items.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
            }
        }
    }
}