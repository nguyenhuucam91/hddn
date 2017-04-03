using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HDNHD.Core.Extensions;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class HoaDonFilterModel : HDNHD.Core.Models.BaseFilterModel<HoaDonModel>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public ELoaiKhachHang? LoaiKhachHang
        {
            get;
            set;
        }

        public EHinhThucThanhToan? HinhThucThanhToan
        {
            get;
            set;
        }

        public bool? TrangThaiThu { get; set; }

        // filter by management info
        public int? QuanHuyenID
        {
            get;
            set;
        }

        public int? ToID
        {
            get;
            set;
        }

        public int? NhanVienID
        {
            get;
            set;
        }

        public int? TuyenKHID
        {
            get;
            set;
        }

        // filter by customer info
        public string MaKH
        {
            get;
            set;
        }

        public string TenKH
        {
            get;
            set;
        }

        public string DiaChiKH
        {
            get;
            set;
        }

        #region BaseFilterModel
        public override void ApplyFilter(ref IQueryable<HoaDonModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);

            if (From != null)
                items = items.Where(m => m.HoaDon.NamHoaDon > From.Value.Year ||
                    (m.HoaDon.NamHoaDon == From.Value.Year && m.HoaDon.ThangHoaDon >= From.Value.Month));

            if (To != null)
                items = items.Where(m => m.HoaDon.NamHoaDon < To.Value.Year ||
                    (m.HoaDon.NamHoaDon == To.Value.Year && m.HoaDon.ThangHoaDon <= To.Value.Month));

            if (TrangThaiThu != null)
            {
                // nullable
                if (TrangThaiThu.Value)
                {
                    items = items.Where(m => m.HoaDon.Trangthaithu == true);
                } else
                {
                    items = items.Where(m => m.HoaDon.Trangthaithu == false || m.HoaDon.Trangthaithu == null);
                }
            }

            if (LoaiKhachHang != null)
            {
                if (LoaiKhachHang == ELoaiKhachHang.CoQuanToChuc)
                {
                    items = items.Where(m => m.KhachHang.LoaiKHID != (int)ELoaiKhachHang.HoGiaDinh);
                }
                else
                {
                    items = items.Where(m => m.KhachHang.LoaiKHID == (int)LoaiKhachHang.Value);
                }
            }

            if (HinhThucThanhToan != null)
            {
                items = items.Where(m => m.KhachHang.HinhthucttID == (int) HinhThucThanhToan.Value);
            }

            //* filter by management info
            if (TuyenKHID != null) // find by TuyenKHID
            {
                items = items.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);
            } else if (NhanVienID != null) // find by NhanVienID
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        where ttnv.NhanVienID == NhanVienID
                        select hdkh;
            }
            else if (ToID != null) // find by ToID
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                        where nv.ToQuanHuyenID == ToID
                        select hdkh;
            }
            else if (QuanHuyenID != null) // find by QuanHuyenID
            {
                items = items.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
            }

            //* filter by customer info
            if (MaKH != null)
            {
                items = items.Where(m => m.KhachHang.MaKhachHang == MaKH);
            }

            if (TenKH != null)
            {
                items = items.Where(m => m.KhachHang.Ten.Contains(TenKH));
            }

            if (DiaChiKH != null)
            {
                items = items.Where(m => m.KhachHang.Diachi.Contains(DiaChiKH));
            }
        }
        #endregion
    }
}