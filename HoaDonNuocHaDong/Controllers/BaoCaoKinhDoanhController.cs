using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Models.BaoCaoInHoaDon;
using HoaDonNuocHaDong.Models.BaoCaoKinhDoanh;
using HoaDonNuocHaDong.Repositories;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class BaoCaoKinhDoanhController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        HoaDonNuocHaDong.Helper.ApGiaHelper apGia = new HoaDonNuocHaDong.Helper.ApGiaHelper();
        HoaDonHaDong.Helper.ChiSo chiSo = new HoaDonHaDong.Helper.ChiSo();

        public ActionResult BaoCaoKinhDoanh()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhanhHangKyHopDongMoi()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangThanhLyHopDong()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangTamNgungCapNuoc()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangCapNuocTroLai()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangTheoTuyenOngKyThuat()
        {
            ViewData["tuyenOng"] = db.Tuyenongs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangTheoDongHoTongCap2()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangTheoTuyenKhachHang()
        {
            List<Tuyenkhachhang> tuyen = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewData["tuyenKH"] = tuyen;
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangTheoNhanVien()
        {
            List<Nhanvien> nhanVien = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewData["nhanvien"] = nhanVien;
            return View();
        }

        public ActionResult XuLyDanhSachKhachHangTheoToQuanLy()
        {
            int phongBanId = getPhongBanNguoiDung();
            List<ToQuanHuyen> ls = db.ToQuanHuyens.Where(p => (p.IsDelete == false || p.IsDelete == null) && p.PhongbanID == phongBanId).ToList();
            ViewData["to"] = ls;
            return View();
        }

        public ActionResult XuLyDanhSachKhachHangTheoDonViQuanLy()
        {
            return View();
        }
        public int getPhongBanNguoiDung()
        {
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            if (nhanVien != null)
            {
                var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
                int phongBanID = phongBan.PhongbanID;
                return phongBanID;
            }
            return 0;
        }

        #region BaoCaoKhachHangApGiaTongHop

        public ActionResult XuLyDanhSachKhachHangLoaiGiaTongHop()
        {
            DateTime d1 = DateTime.Now;
            ViewBag.dt1 = d1;
            ViewBag.soLuongKhachHangApTongHop = filterDanhSachKhachHangApGiaTongHop().Count();
            return View("Danhsachkhachhangloaigiatonghop");
        }

        public JsonResult jsonTransformKhachHangApGiaTongHop()
        {
            var khachHangApTongHop = filterDanhSachKhachHangApGiaTongHop();
            List<DanhSachKhachHangApGiaChung> dsApTongHop = createDanhSachKhachHangApGiaTongHop(khachHangApTongHop);
            return Json(dsApTongHop);
        }

        public List<DanhSachKhachHangApGiaChung> filterDanhSachKhachHangApGiaTongHop()
        {
            var khachHang = (from i in db.Khachhangs
                             join s in db.Apgiatonghops on i.KhachhangID equals s.KhachhangID
                             join t in db.Tuyenkhachhangs on i.TuyenKHID equals t.TuyenKHID
                             where i.LoaiapgiaID == KhachHang.TONGHOP
                             select new DanhSachKhachHangApGiaChung
                             {
                                 MaKH = i.MaKhachHang,
                                 HoTen = i.Ten,
                                 DiaChi = i.Diachi,
                                 Tuyen = t.Matuyen,
                                 TTDoc = i.TTDoc,
                                 CachTinh = s.CachTinh == 1 ? "Phần trăm" : "Số khoán",
                                 KhachHangID = i.KhachhangID,
                             }).Distinct().OrderBy(p => p.Tuyen).ThenBy(p => p.TTDoc).ToList();
            return khachHang;
        }

        public List<DanhSachKhachHangApGiaChung> createDanhSachKhachHangApGiaTongHop(List<DanhSachKhachHangApGiaChung> dsKhachHang)
        {
            List<DanhSachKhachHangApGiaChung> dsApGiaChung = new List<DanhSachKhachHangApGiaChung>();
            int i = 1;
            foreach (var item in dsKhachHang)
            {
                DanhSachKhachHangApGiaChung khachHang = new DanhSachKhachHangApGiaChung();
                khachHang.STT = i;
                khachHang.KhachHangID = item.KhachHangID;
                khachHang.MaKH = item.MaKH;
                khachHang.HoTen = item.HoTen;
                khachHang.DiaChi = item.DiaChi;
                khachHang.Tuyen = item.Tuyen;
                khachHang.TTDoc = item.TTDoc;
                khachHang.CachTinh = item.CachTinh;
                khachHang.SinhHoat = apGia.getChiSoApGiaTongHop(item.KhachHangID, HoaDonNuocHaDong.Helper.KhachHang.SINHHOAT);
                khachHang.SanXuat = apGia.getChiSoApGiaTongHop(item.KhachHangID, HoaDonNuocHaDong.Helper.KhachHang.SANXUAT);
                khachHang.CongCong = apGia.getChiSoApGiaTongHop(item.KhachHangID, HoaDonNuocHaDong.Helper.KhachHang.COQUANHANHCHINH);
                khachHang.KinhDoanh = apGia.getChiSoApGiaTongHop(item.KhachHangID, HoaDonNuocHaDong.Helper.KhachHang.KINHDOANHDICHVU);
                dsApGiaChung.Add(khachHang);
                i++;
            }
            return dsApGiaChung;
        }

        #endregion

        #region DanhSachKhachHangApGiaDacBiet
        public ActionResult XuLyDanhSachKhachHangLoaiGiaDacBiet()
        {
            DateTime d1 = DateTime.Now;
            ViewBag.dt1 = d1;
            ViewBag.soLuongKhachHangApGiaDacBiet = filterDanhSachKhachHangLoaiGiaDacBiet().Count();
            return View("Danhsachkhachhangloaigiadacbiet");
        }

        public List<DanhSachKhachHangApGiaChung> filterDanhSachKhachHangLoaiGiaDacBiet()
        {
            var khachHang = (from i in db.Khachhangs
                             join r in db.Hoadonnuocs on i.KhachhangID equals r.KhachhangID
                             join s in db.ApGiaDacBiets on r.HoadonnuocID equals s.HoaDonNuocID
                             join t in db.Tuyenkhachhangs on i.TuyenKHID equals t.TuyenKHID
                             where r.ThangHoaDon == DateTime.Now.Month && r.NamHoaDon == DateTime.Now.Year
                             select new DanhSachKhachHangApGiaChung
                             {
                                 MaKH = i.MaKhachHang,
                                 HoTen = i.Ten,
                                 DiaChi = i.Diachi,
                                 Tuyen = t.Matuyen,
                                 TTDoc = i.TTDoc,
                                 KhachHangID = i.KhachhangID,
                                 SH1 = s.SH1.ToString(),
                                 SH2 = s.SH2.ToString(),
                                 SH3 = s.SH3.ToString(),
                                 SH4 = s.SH4.ToString(),
                                 KinhDoanh = s.KDDV.ToString(),
                                 SanXuat = s.SXSD.ToString(),
                                 HanhChinh = s.HC.ToString(),
                                 CongCong = s.CC.ToString()
                             }).Distinct().OrderBy(p => p.TTDoc).ToList();
            return khachHang;
        }

        public JsonResult getDanhSachKhachHangApGiaDacBietToJson()
        {
            List<DanhSachKhachHangApGiaChung> danhSachKhachHangLoaiGiaDacBiet = filterDanhSachKhachHangLoaiGiaDacBiet();
            return Json(danhSachKhachHangLoaiGiaDacBiet);
        }

        #endregion
        public ActionResult XuLyDanhSachKhachHangCacLoaiGiaKhac()
        {
            return View();
        }

        public ActionResult XuLyDanhSachKhachHangCoGhiChu()
        {
            DateTime d1 = DateTime.Now;
            ControllerBase<DanhSachKhachHangCoGhiChu> cb = new ControllerBase<DanhSachKhachHangCoGhiChu>();
            List<DanhSachKhachHangCoGhiChu> lst = cb.Query(
                "BC14"
                );

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            return View("Danhsachkhachhangcoghichu");

        }

        public ActionResult XuLyDanhSachKhachHangTheoDinhMuc()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangHetHanDinhMuc()
        {
            return View();
        }
        public ActionResult XuLyDanhSachKhachHangCoSanLuongDotBien()
        {
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ControllerBase<DanhSachKhachHangCoSanLuongDotBien> cb = new ControllerBase<DanhSachKhachHangCoSanLuongDotBien>();
            List<DanhSachKhachHangCoSanLuongDotBien> lst = cb.Query(
                "BC17",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            return View("Danhsachkhachhangcosanluongdotbien");
        }
        public ActionResult XuLyDanhSachKhachHangKhongSanLuong()
        {
            return View();
        }
        public ActionResult XuLyBaoCaoTongHopSanLuong()
        {
            ViewData["xinghieps"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            ViewData["tuyens"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).ToList();
            ViewBag.selectedMonth = DateTime.Now.Month;
            ViewBag.selectedYear = DateTime.Now.Year;
            return View();
        }
        public ActionResult XuLyBaoCaoTongHopKhachHangQuanLy()
        {
            return View();
        }
        public ActionResult XuLyBaoCaoThatThoat()
        {
            return View();
        }
        // GET: /BaoCaoKinhDoanh/
        // lấy ra những khách hàng có ngày ký hợp đồng trong khoảng startdate đến enddate
        // BC1
        [HttpPost]
        public ActionResult DanhSachKhanhHangKyHopDongMoi(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), int.Parse(fc["d1"]));
            DateTime d2 = new DateTime(int.Parse(fc["y2"]), int.Parse(fc["m2"]), int.Parse(fc["d2"]));
            ControllerBase<DanhSachKhachHangKyHopDongMoi> cb = new ControllerBase<DanhSachKhachHangKyHopDongMoi>();
            List<DanhSachKhachHangKyHopDongMoi> lst = cb.Query(
                "BC1",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangThanhLyHopDong(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), int.Parse(fc["d1"]));
            DateTime d2 = new DateTime(int.Parse(fc["y2"]), int.Parse(fc["m2"]), int.Parse(fc["d2"]));
            ControllerBase<DanhSachKhachHangThanhLyHopDong> cb = new ControllerBase<DanhSachKhachHangThanhLyHopDong>();
            List<DanhSachKhachHangThanhLyHopDong> lst = cb.Query(
                "BC2",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTamNgungCapNuoc(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), int.Parse(fc["d1"]));
            DateTime d2 = new DateTime(int.Parse(fc["y2"]), int.Parse(fc["m2"]), int.Parse(fc["d2"]));
            ControllerBase<DanhSachKhachHangTamNgungCapNuoc> cb = new ControllerBase<DanhSachKhachHangTamNgungCapNuoc>();
            List<DanhSachKhachHangTamNgungCapNuoc> lst = cb.Query(
                "BC3",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangCapNuocTroLai(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), int.Parse(fc["d1"]));
            DateTime d2 = new DateTime(int.Parse(fc["y2"]), int.Parse(fc["m2"]), int.Parse(fc["d2"]));
            ControllerBase<DanhSachKhachHangCapNuocTroLai> cb = new ControllerBase<DanhSachKhachHangCapNuocTroLai>();
            List<DanhSachKhachHangCapNuocTroLai> lst = cb.Query(
                "BC4",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoTuyenOngKyThuat(FormCollection fc)
        {
            string d1 = fc["d1"];
            ControllerBase<DanhSachKhachHangTheoTuyenOngKyThuat> cb = new ControllerBase<DanhSachKhachHangTheoTuyenOngKyThuat>();
            List<DanhSachKhachHangTheoTuyenOngKyThuat> lst = cb.Query(
                "BC5",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            int tuyenongID = Convert.ToInt32(d1);
            String tenTuyenOng = db.Tuyenongs.FirstOrDefault(p => p.TuyenongID == tuyenongID).Tentuyen;
            ViewBag.dt1 = tenTuyenOng;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoDongHoTongCap2(FormCollection fc)
        {
            string d1 = fc["d1"];
            ControllerBase<DanhSachKhachHangTheoDongHoTongCap2> cb = new ControllerBase<DanhSachKhachHangTheoDongHoTongCap2>();
            List<DanhSachKhachHangTheoDongHoTongCap2> lst = cb.Query(
                "BC6",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoTuyenKhachHang(FormCollection fc)
        {
            int d1 = Convert.ToInt32(fc["d1"]);
            ControllerBase<DanhSachKhachHangTheoTuyenKhachHang> cb = new ControllerBase<DanhSachKhachHangTheoTuyenKhachHang>();
            List<DanhSachKhachHangTheoTuyenKhachHang> lst = cb.Query(
                "BC7",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            var tenTuyen = db.Tuyenkhachhangs.Find(d1).Ten;
            ViewBag.dt1 = tenTuyen;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoNhanVien(FormCollection fc)
        {
            string d1 = fc["d1"];
            ControllerBase<DanhSachKhachHangTheoNhanVien> cb = new ControllerBase<DanhSachKhachHangTheoNhanVien>();
            List<DanhSachKhachHangTheoNhanVien> lst = cb.Query(
                "BC8",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            int nhanVienID = Convert.ToInt32(d1);
            Nhanvien nV = db.Nhanviens.FirstOrDefault(p => p.NhanvienID == nhanVienID);
            ViewBag.dt1 = nV.MaNhanVien + "-" + nV.Ten;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoToQuanLy(FormCollection fc)
        {
            string d1 = fc["d1"];
            int num = Convert.ToInt32(d1);

            ControllerBase<DanhSachKhachHangTheoToQuanLy> cb = new ControllerBase<DanhSachKhachHangTheoToQuanLy>();
            List<DanhSachKhachHangTheoToQuanLy> lst = cb.Query(
                "BC9",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            ViewBag.dt1 = db.ToQuanHuyens.Find(num).Ma;

            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangTheoDonViQuanLy(FormCollection fc)
        {
            string d1 = fc["d1"];
            ControllerBase<DanhSachKhachHangTheoDonViQuanLy> cb = new ControllerBase<DanhSachKhachHangTheoDonViQuanLy>();
            List<DanhSachKhachHangTheoDonViQuanLy> lst = cb.Query(
                "BC10",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;

            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangLoaiGiaTongHop(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), 1);
            var khachHang = (from i in db.Khachhangs
                             join s in db.Apgiatonghops on i.KhachhangID equals s.KhachhangID
                             join t in db.Tuyenkhachhangs on i.TuyenKHID equals t.TuyenKHID
                             where i.LoaiapgiaID == 7 && i.Ngaykyhopdong.Value.Month == d1.Month && i.Ngaykyhopdong.Value.Year == d1.Year
                             select new
                             {
                                 MaKH = i.MaKhachHang,
                                 HoTen = i.Ten,
                                 DiaChi = i.Diachi,
                                 Tuyen = t.Matuyen,
                                 TTDoc = i.TTDoc,
                                 CachTinh = s.CachTinh == 1 ? "Phần trăm" : "Số khoán",
                                 KhachHangID = i.KhachhangID
                             }).Distinct().OrderBy(p => p.TTDoc).ToList();
            ViewData["khachHang"] = khachHang;
            ViewBag.dt1 = d1;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangLoaiGiaDacBiet(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), 1);
            var khachHang = (from i in db.Khachhangs
                             join s in db.Apgiatonghops on i.KhachhangID equals s.KhachhangID
                             join t in db.Tuyenkhachhangs on i.TuyenKHID equals t.TuyenKHID
                             where i.LoaiapgiaID == 8 && i.Ngaykyhopdong.Value.Month == d1.Month && i.Ngaykyhopdong.Value.Year == d1.Year
                             select new
                             {
                                 MaKH = i.MaKhachHang,
                                 HoTen = i.Ten,
                                 DiaChi = i.Diachi,
                                 Tuyen = t.Matuyen,
                                 TTDoc = i.TTDoc,
                                 KhachHangID = i.KhachhangID
                             }).Distinct().OrderBy(p => p.TTDoc).ToList();
            ViewData["khachHang"] = khachHang;
            ViewBag.dt1 = d1;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangCacLoaiGiaKhac(FormCollection fc)
        {

            DateTime d1 = DateTime.Now;
            int loaiApGiaID = Convert.ToInt32(fc["d2"]);
            ControllerBase<DanhSachKhachHangCacLoaiGiaKhac> cb = new ControllerBase<DanhSachKhachHangCacLoaiGiaKhac>();
            List<DanhSachKhachHangCacLoaiGiaKhac> lst = cb.Query(
                "BC13",
                new SqlParameter("@d2", loaiApGiaID));

            ViewData["lst"] = lst;
            ViewBag.ten = db.Loaiapgias.FirstOrDefault(p => p.LoaiapgiaID == loaiApGiaID).Ten;
            ViewBag.dt1 = d1;
            return View();
        }

        [HttpPost]
        public ActionResult DanhSachKhachHangCoGhiChu(FormCollection fc)
        {
            DateTime d1 = DateTime.Now;
            ControllerBase<DanhSachKhachHangCoGhiChu> cb = new ControllerBase<DanhSachKhachHangCoGhiChu>();
            List<DanhSachKhachHangCoGhiChu> lst = cb.Query(
                "BC14",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            return View();
        }

        [HttpPost]
        public ActionResult DanhSachKhachHangTheoDinhMuc(FormCollection fc)
        {
            int d1 = int.Parse(fc["d1"]);
            int d2 = int.Parse(fc["d2"]);
            DateTime d3 = DateTime.Now;

            ControllerBase<DanhSachKhachHangTheoDinhMuc> cb = new ControllerBase<DanhSachKhachHangTheoDinhMuc>();
            List<DanhSachKhachHangTheoDinhMuc> lst = cb.Query(
                "BC15",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2)
                );

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            ViewBag.dt3 = d3;
            return View();
        }

        [HttpPost]
        public ActionResult DanhSachKhachHangHetHanDinhMuc(FormCollection fc)
        {
            int d1 = int.Parse(fc["d1"]);
            int d2 = int.Parse(fc["d2"]);
            DateTime ngayHetAp = DateTime.Parse(fc["Ngayhetap"]);
            DateTime d3 = DateTime.Now;
            ControllerBase<DanhSachKhachHangHetHanDinhMuc> cb = new ControllerBase<DanhSachKhachHangHetHanDinhMuc>();
            List<DanhSachKhachHangHetHanDinhMuc> lst = cb.Query(
                "BC16",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2),
                new SqlParameter("@hethandinhmuc", ngayHetAp)
                );

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            ViewBag.dt3 = d3;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangCoSanLuongDotBien(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), 1);
            ControllerBase<DanhSachKhachHangCoSanLuongDotBien> cb = new ControllerBase<DanhSachKhachHangCoSanLuongDotBien>();
            List<DanhSachKhachHangCoSanLuongDotBien> lst = cb.Query(
                "BC17",
                new SqlParameter("@d1", d1));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            return View();
        }
        [HttpPost]
        public ActionResult DanhSachKhachHangKhongSanLuong(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), 1);
            DateTime d2 = new DateTime(int.Parse(fc["y2"]), int.Parse(fc["m2"]), 28);
            ControllerBase<DanhSachKhachHangKhongSanLuong> cb = new ControllerBase<DanhSachKhachHangKhongSanLuong>();
            List<DanhSachKhachHangKhongSanLuong> lst = cb.Query(
                "BC18",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));

            ViewData["lst"] = lst;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }

        [HttpPost]
        public ActionResult BaoCaoTongHopSanLuong(FormCollection fc, int type)
        {

            int monthReceipt = !String.IsNullOrEmpty(fc["m1"]) ? Convert.ToInt32(fc["m1"]) : DateTime.Now.Month;
            int yearReceipt = !String.IsNullOrEmpty(fc["y1"]) ? Convert.ToInt32(fc["y1"]) : DateTime.Now.Year;

            //tuyến ống
            if (type == 0)
            {
                List<BaoCaoTongHopSanLuong> ls = xemBaoCaoSanLuongTheoTuyenOng(monthReceipt, yearReceipt);
                ViewBag.columnTitle = "Tuyến ống";
                ViewBag.tong = ls;
            }
            //tuyến
            else if (type == 1)
            {
                String tuyens = !String.IsNullOrEmpty(fc["tuyen"]) ? fc["tuyen"] : "";
                List<BaoCaoTongHopSanLuong> ls = xemBaoCaoSanLuongTheoTuyenKH(monthReceipt, yearReceipt, tuyens);
                ViewBag.columnTitle = "Tuyến khách hàng";
                ViewBag.tong = ls;
            }
            //nhân viên
            else if (type == 2)
            {
                List<BaoCaoTongHopSanLuong> ls = xemBaoCaoSanLuongTheoNhanvien(monthReceipt, yearReceipt);
                ViewBag.columnTitle = "Nhân viên";
                ViewBag.tong = ls;
            }
            //tổ kĩ thuật
            else if (type == 3)
            {
                String xinghieps = !String.IsNullOrEmpty(fc["xinghiep"]) ? fc["xinghiep"] : "";
                List<BaoCaoTongHopSanLuong> ls = xemBaoCaoSanLuongTheoToQuanHuyen(monthReceipt, yearReceipt, xinghieps);
                ViewBag.columnTitle = "Tổ quản lí";
                ViewBag.tong = ls;
            }

            ViewBag.monthReceipt = monthReceipt;
            ViewBag.yearReceipt = yearReceipt;
            return View();
        }

        public List<BaoCaoTongHopSanLuong> xemBaoCaoSanLuongTheoTuyenOng(int month, int year)
        {
            ControllerBase<BaoCaoTongHopSanLuong> cb = new ControllerBase<BaoCaoTongHopSanLuong>();
            List<BaoCaoTongHopSanLuong> lst = cb.Query("BaoCaoTongHopSanLuongTheoTuyenOng",
                new SqlParameter("@d1", month),
                new SqlParameter("@d2", year),
                new SqlParameter("@SH1Price", chiSo.getSoTienTheoApGia("SH1")),
                new SqlParameter("@SH2Price", chiSo.getSoTienTheoApGia("SH2")),
                new SqlParameter("@SH3Price", chiSo.getSoTienTheoApGia("SH3")),
                new SqlParameter("@SH4Price", chiSo.getSoTienTheoApGia("SH4")),
                new SqlParameter("@CCPrice", chiSo.getSoTienTheoApGia("CC")),
                new SqlParameter("@HCPrice", chiSo.getSoTienTheoApGia("HC")),
                new SqlParameter("@SXXDPrice", chiSo.getSoTienTheoApGia("SXXD")),
                new SqlParameter("@KDDVPrice", chiSo.getSoTienTheoApGia("KDDV"))
                );
            return lst;
        }

        public List<BaoCaoTongHopSanLuong> xemBaoCaoSanLuongTheoTuyenKH(int month, int year, String tuyens)
        {
            ControllerBase<BaoCaoTongHopSanLuong> cb = new ControllerBase<BaoCaoTongHopSanLuong>();
            List<BaoCaoTongHopSanLuong> lst = cb.Query("BaoCaoTongHopSanLuongTheoTuyenKhachHang",
                new SqlParameter("@d1", month),
                new SqlParameter("@d2", year),
                new SqlParameter("@SH1Price", chiSo.getSoTienTheoApGia("SH1")),
                new SqlParameter("@SH2Price", chiSo.getSoTienTheoApGia("SH2")),
                new SqlParameter("@SH3Price", chiSo.getSoTienTheoApGia("SH3")),
                new SqlParameter("@SH4Price", chiSo.getSoTienTheoApGia("SH4")),
                new SqlParameter("@CCPrice", chiSo.getSoTienTheoApGia("CC")),
                new SqlParameter("@HCPrice", chiSo.getSoTienTheoApGia("HC")),
                new SqlParameter("@SXXDPrice", chiSo.getSoTienTheoApGia("SXXD")),
                new SqlParameter("@KDDVPrice", chiSo.getSoTienTheoApGia("KDDV")),
                new SqlParameter("@list", tuyens)
                );
            return lst;
        }

        public List<BaoCaoTongHopSanLuong> xemBaoCaoSanLuongTheoNhanvien(int month, int year)
        {
            ControllerBase<BaoCaoTongHopSanLuong> cb = new ControllerBase<BaoCaoTongHopSanLuong>();
            List<BaoCaoTongHopSanLuong> lst = cb.Query("BaoCaoTongHopSanLuongTheoNhanvien",
                new SqlParameter("@d1", month),
                new SqlParameter("@d2", year),
                new SqlParameter("@SH1Price", chiSo.getSoTienTheoApGia("SH1")),
                new SqlParameter("@SH2Price", chiSo.getSoTienTheoApGia("SH2")),
                new SqlParameter("@SH3Price", chiSo.getSoTienTheoApGia("SH3")),
                new SqlParameter("@SH4Price", chiSo.getSoTienTheoApGia("SH4")),
                new SqlParameter("@CCPrice", chiSo.getSoTienTheoApGia("CC")),
                new SqlParameter("@HCPrice", chiSo.getSoTienTheoApGia("HC")),
                new SqlParameter("@SXXDPrice", chiSo.getSoTienTheoApGia("SXXD")),
                new SqlParameter("@KDDVPrice", chiSo.getSoTienTheoApGia("KDDV"))
                );
            return lst;
        }

        public List<BaoCaoTongHopSanLuong> xemBaoCaoSanLuongTheoToQuanHuyen(int month, int year, String xinghieps)
        {
            ControllerBase<BaoCaoTongHopSanLuong> cb = new ControllerBase<BaoCaoTongHopSanLuong>();
            List<BaoCaoTongHopSanLuong> lst = cb.Query("BaoCaoTongHopSanLuongTheoToQuanLi",
                new SqlParameter("@d1", month),
                new SqlParameter("@d2", year),
                new SqlParameter("@SH1Price", chiSo.getSoTienTheoApGia("SH1")),
                new SqlParameter("@SH2Price", chiSo.getSoTienTheoApGia("SH2")),
                new SqlParameter("@SH3Price", chiSo.getSoTienTheoApGia("SH3")),
                new SqlParameter("@SH4Price", chiSo.getSoTienTheoApGia("SH4")),
                new SqlParameter("@CCPrice", chiSo.getSoTienTheoApGia("CC")),
                new SqlParameter("@HCPrice", chiSo.getSoTienTheoApGia("HC")),
                new SqlParameter("@SXXDPrice", chiSo.getSoTienTheoApGia("SXXD")),
                new SqlParameter("@KDDVPrice", chiSo.getSoTienTheoApGia("KDDV")),
                new SqlParameter("@list", xinghieps)
                );
            return lst;
        }

        [HttpPost]
        public ActionResult BaoCaoTongHopKhachHangQuanLy(FormCollection fc)
        {
            DateTime d1 = DateTime.Parse(fc["d1"]);
            DateTime d2 = DateTime.Parse(fc["d2"]);
            ControllerBase<BaoCaoTongHopKhachHangQuanLy> cb = new ControllerBase<BaoCaoTongHopKhachHangQuanLy>();
            List<BaoCaoTongHopKhachHangQuanLy> lst = cb.Query(
                "BC20",
                new SqlParameter("@d1", d1),
                new SqlParameter("@d2", d2));
            int[] arrSum = new int[12];
            lst.ForEach(x =>
            {
                arrSum[0] += x.DongHoThongKeDauKy;
                arrSum[1] += x.KhoanThongKeDauKy;
                arrSum[2] += x.TongThongKeDauKy;
                arrSum[3] += x.DongHoTangTrongKy;
                arrSum[4] += x.KhoanTangTrongKy;
                arrSum[5] += x.TongTangTrongKy;
                arrSum[6] += x.DongHoGiamTrongKy;
                arrSum[7] += x.KhoanGiamTrongKy;
                arrSum[8] += x.TongGiamTrongKy;
                arrSum[9] += x.DongHoThongKeCuoiKy;
                arrSum[10] += x.KhoanThongKeCuoiKy;
                arrSum[11] += x.TongThongKeCuoiKy;
            });
            ViewData["lst"] = lst;
            ViewData["tong"] = arrSum;
            ViewBag.dt1 = d1;
            ViewBag.dt2 = d2;
            return View();
        }

        [HttpPost]
        public ActionResult BaoCaoThatThoat(FormCollection fc)
        {
            DateTime d1 = new DateTime(int.Parse(fc["y1"]), int.Parse(fc["m1"]), 1);
            ControllerBase<BaoCaoThatThoat> cb = new ControllerBase<BaoCaoThatThoat>();
            List<BaoCaoThatThoat> lst = cb.Query(
                "BC21",
                new SqlParameter("@d1", d1));
            int[] arrSum = new int[6];
            lst.ForEach(x =>
            {
                arrSum[0] += x.SoKH;
                arrSum[1] += x.SanLuongThangBaoCao;
                arrSum[2] += x.SanLuongThangTruoc;
                arrSum[3] += x.SanLuongPhat;
                arrSum[4] += x.LuongThatThoat;
                arrSum[5] += x.TiLeThatThoat;
            });
            ViewData["lst"] = lst;
            ViewData["tong"] = arrSum;
            ViewBag.dt1 = d1;
            return View();
        }
    }
}