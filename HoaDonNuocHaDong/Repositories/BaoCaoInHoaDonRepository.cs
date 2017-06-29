using HoaDonNuocHaDong.Models.BaoCaoInHoaDon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class BaoCaoInHoaDonRepository
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public List<BaoCaoHoaDonNhan> getDanhSachHoaDonNhanTatCaCacTuyen(DateTime createdDate, DateTime endDate)
        {
            var lsHoaDon = (from i in db.Hoadonnuocs
                            join r in db.Lichsuhoadons on i.HoadonnuocID equals r.HoaDonID
                            join t in db.Khachhangs on i.KhachhangID equals t.KhachhangID
                            where i.Ngayhoadon >= createdDate && i.Ngayhoadon <= endDate && i.Tongsotieuthu > 0
                            select new BaoCaoHoaDonNhan
                            {
                                ID = i.HoadonnuocID,
                                MaKH = r.MaKH,
                                LoaiKH = t.LoaiKHID,
                                NgayHoaDon = i.Ngayhoadon,
                                TenKH = r.TenKH,
                                TongTien = r.TongCong,
                                TuyenKHID = t.TuyenKHID,
                                DiaChi = t.Diachi
                            }).ToList();
            return lsHoaDon;
        }

        public List<BaoCaoHoaDonNhan> getDanhSachHoaDonNhanThuocTuyen(int tuyenID, DateTime createdDate, DateTime endDate)
        {
            var lsHoaDon = (from i in db.Hoadonnuocs
                            join r in db.Lichsuhoadons on i.HoadonnuocID equals r.HoaDonID
                            join t in db.Khachhangs on i.KhachhangID equals t.KhachhangID
                            where i.Ngayhoadon >= createdDate && i.Ngayhoadon <= endDate && i.Tongsotieuthu > 0 && t.TuyenKHID == tuyenID
                            select new BaoCaoHoaDonNhan
                            {
                                ID = i.HoadonnuocID,
                                MaKH = r.MaKH,
                                LoaiKH = t.LoaiKHID,
                                NgayHoaDon = i.Ngayhoadon,
                                TenKH = r.TenKH,
                                TongTien = r.TongCong,
                                TuyenKHID = t.TuyenKHID,
                                DiaChi = t.Diachi
                            }).ToList();
            return lsHoaDon;
        }

        public List<Models.Hoadonnuocbihuy.Hoadonnuocbihuy> getHoaDonsBiHuy()
        {

            var huyhd = (from i in db.Hoadonnuocbihuys
                         join b in db.Hoadonnuocs on i.HoadonnuocID equals b.HoadonnuocID
                         join t in db.SoTienNopTheoThangs on i.HoadonnuocID equals t.HoaDonNuocID
                         join s in db.Khachhangs on b.KhachhangID equals s.KhachhangID
                         join r in db.Tuyenkhachhangs on s.TuyenKHID equals r.TuyenKHID
                         select new Models.Hoadonnuocbihuy.Hoadonnuocbihuy
                         {
                             id = i.Id,
                             SoHoaDon = i.Sohieuhoadon,
                             maKH = s.MaKhachHang,
                             tenKH = s.Ten,
                             SoTien = t.SoTienPhaiNop,
                             Tuyen = r.Matuyen,
                             ngayHuy = i.Ngayhuyhoadon.ToString(),
                             NguoiYeuCauHuy = i.Nguoiyeucauhuy,
                             lidohuy = i.Lidohuyhoadon,
                             nguoiHuy = i.Nguoihuyhoadon.Value,
                             ThangHuyHoaDon = b.ThangHoaDon.Value.ToString(),
                             NamHuyHoaDon = b.NamHoaDon.Value.ToString()
                         }).ToList();
            return huyhd;
        }
    }
}