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
                items = items.Where(m => m.HoaDon.Trangthaithu == TrangThaiThu.Value);

            // join with KhachHang
            items = from hdm in items
                    join kh in context.Khachhangs on hdm.HoaDon.KhachhangID equals kh.KhachhangID
                    select new HoaDonModel()
                    {
                        HoaDon = hdm.HoaDon,
                        KhachHang = kh
                    };

            if (LoaiKhachHang != null)
                items = items.Where(m => m.KhachHang.LoaiKHID == (int)LoaiKhachHang.Value);

            if (HinhThucThanhToan != null)
                items = items.Where(m => m.KhachHang.HinhthucttID == (int)EHinhThucThanhToan.ChuyenKhoan);

            //* filter by management info
            // find by NhanVienID
            if (NhanVienID != null) 
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        where ttnv.NhanVienID == NhanVienID
                        select hdkh;
            }
            else if (TuyenKHID != null) // find by TuyenKHID
            {
                items = items.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);   
            } else if (ToID != null) // find by ToID
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                        where nv.ToQuanHuyenID == ToID
                        select hdkh;
            }
            else if (QuanHuyenID != null) // find by QuanHuyenID
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                        join to in context.ToQuanHuyens on nv.ToQuanHuyenID equals to.ToQuanHuyenID
                        where to.QuanHuyenID == QuanHuyenID
                        select hdkh;
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

            // join SoTienNopTheoThang
            items = from item in items
                    join stntt in context.SoTienNopTheoThangs
                    on item.HoaDon.SoTienNopTheoThangID equals stntt.ID
                    select new HoaDonModel()
                    {
                        HoaDon = item.HoaDon,
                        KhachHang = item.KhachHang,
                        SoTienNopTheoThang = stntt
                    };
        }
        #endregion
    }
}