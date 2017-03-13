using HoaDonNuocHaDong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using HoaDonNuocHaDong.Base;

namespace HoaDonNuocHaDong.Controllers
{
    public class BaoCaoController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        
        //
        // GET: /BaoCao/
        //Báo cáo tổng hợp doanh thu sản lượng theo từng tuyến khách hàng
        public ActionResult BaoCaoTongHopDoanhThuSanLuongTheoTuyen()
        {
            List<BaoCaoTongHopDoanhThuSanLuongTheoTuyen> lstBCTH = new List<BaoCaoTongHopDoanhThuSanLuongTheoTuyen>();
            BaoCaoTongHopDoanhThuSanLuongTheoTuyen tong = new BaoCaoTongHopDoanhThuSanLuongTheoTuyen() {SoLuongHoaDon=0,TongSanLuong=0,SH1=0,SH2=0,SH3=0,SH4=0,SX=0,KDDV=0,HCSN=0,DTTT=0,CC=0,VAT=0,TongCong=0,PhiNuocThai=0 };
            foreach (var hoadon in db.Hoadonnuocs.OrderBy(p => p.HoadonnuocID).ToList())
            {
                //tạo ra goa tri cho tung tuyen
                BaoCaoTongHopDoanhThuSanLuongTheoTuyen bcth = new BaoCaoTongHopDoanhThuSanLuongTheoTuyen();
                bcth.Ma = hoadon.Khachhang.Tuyenkhachhang.TuyenKHID;
                bcth.Ten = hoadon.Khachhang.Tuyenkhachhang.Ten;
                bcth.SoLuongHoaDon = db.Hoadonnuocs.Where(p => p.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Count();
                bcth.TongSanLuong = db.Hoadonnuocs.Where(p => p.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.Tongsotieuthu);
                bcth.SH1 = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.SH1);
                bcth.SH2 = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.SH2);
                bcth.SH3 = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.SH3);
                bcth.SH4 = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.SH4);
                bcth.SX = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.SXXD);
                bcth.CC = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.CC);
                bcth.HCSN = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.HC);
                bcth.KDDV = db.Chitiethoadonnuocs.Where(p => p.Hoadonnuoc.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => c.KDDV);
                bcth.DTTT = bcth.SH1 * 2500 + bcth.SH2 * 3000 + bcth.SH3 * 4000 + bcth.SH4 * 4500 + bcth.SX * 5000 + bcth.CC * 3500 + bcth.HCSN * 4000 + bcth.KDDV * 8500;
                bcth.VAT = bcth.DTTT * 10 / 100;
                bcth.PhiNuocThai = db.Hoadonnuocs.Where(p => p.Khachhang.Tuyenkhachhang.TuyenKHID == bcth.Ma).Sum(c => hoadon.Khachhang.Phibaovemoitruong);
                bcth.TongCong = bcth.PhiNuocThai + bcth.VAT + bcth.DTTT;
                //add vao list bao cao tong hop
                lstBCTH.Add(bcth);

                // cong don gia tri vao tong
                tong.SoLuongHoaDon += bcth.SoLuongHoaDon;
                tong.TongSanLuong += bcth.TongSanLuong;
                tong.SH1 += bcth.SH1;
                tong.SH2 += bcth.SH2;
                tong.SH3 += bcth.SH3;
                tong.SH4 += bcth.SH4;
                tong.SX += bcth.SX;
                tong.CC+= bcth.CC;
                tong.HCSN += bcth.HCSN;
                tong.KDDV += bcth.KDDV;
                tong.DTTT += bcth.DTTT;
                tong.PhiNuocThai += bcth.PhiNuocThai;
                tong.VAT += bcth.VAT;
                tong.TongCong+= bcth.TongCong;
            }
            ViewBag.bcth = lstBCTH;
            ViewBag.tong = tong;
            return View();
        }
        //Báo cáo sản lượng doanh thu theo từng mức áp giá theo từng tháng
        public ActionResult BaoCaoSanLuongDoanhThu()
        {
            List<BaoCaoSanLuongDoanhThu> lstBCSL = new List<BaoCaoSanLuongDoanhThu>();
            BaoCaoSanLuongDoanhThu tong = new BaoCaoSanLuongDoanhThu() {SanLuong=0,DoanhThuTruocThue=0,VAT=0,PhiNuocThai=0,TongCong=0 };
            foreach (var apgia in db.Apgias.OrderBy(p => p.ApgiaID).ToList())
            {
                BaoCaoSanLuongDoanhThu bcsl = new Models.BaoCaoSanLuongDoanhThu();
                bcsl.STT = apgia.ApgiaID;
                bcsl.CacMuc = apgia.Ten;
                switch(bcsl.CacMuc)
                {
                    case "SH1":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.SH1);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 2500;
                            break;
                        }
                    case "SH2":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.SH2);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 3000;
                            break;
                        }
                    case "SH3":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.SH3);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 4000;
                            break;
                        }
                    case "SH4":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.SH4);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 4500;
                            break;
                        }
                    case "SX - XD":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.SXXD);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 5000;
                            break;
                        }
                    case "CC":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.CC);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 3500;
                            break;
                        }
                    case "HC":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.HC);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 4000;
                            break;
                        }
                    case "KD-DV":
                        {
                            bcsl.SanLuong = db.Chitiethoadonnuocs.Sum(c => c.KDDV);
                            bcsl.DoanhThuTruocThue = bcsl.SanLuong * 8500;
                            break;
                        }
                }
                bcsl.VAT = bcsl.DoanhThuTruocThue * 10 / 100;
                bcsl.PhiNuocThai = 0;
                bcsl.TongCong = bcsl.DoanhThuTruocThue + bcsl.PhiNuocThai + bcsl.VAT;
                //add vao list bao cao tong hop
                lstBCSL.Add(bcsl);

                // cong don gia tri vao tong
                tong.SanLuong+=bcsl.SanLuong;
                tong.DoanhThuTruocThue += bcsl.DoanhThuTruocThue;
                tong.VAT += bcsl.VAT;
                tong.PhiNuocThai += bcsl.PhiNuocThai;
                tong.TongCong += bcsl.TongCong;
            }
            ViewBag.bcsl = lstBCSL;
            ViewBag.tong = tong;
            return View();
        }
        //Báo cáo lịch sử dùng nước của 1 khách hàng trong 1 năm
        public ActionResult LichSuDungNuocCuaKhachHang()
        {
            var KhID = Convert.ToInt32(Request.QueryString["id"]);
            var nam = DateTime.Now.Year;
            if (Request.QueryString["year"] != null && Request.QueryString["year"].Length>0)
            {
                nam = Convert.ToInt32(Request.QueryString["year"]);
            }
            // var hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.Ngayhoadon.Value.Month == month && h.Ngayhoadon.Value.Year == year);
            var a = db.Chitiethoadonnuocs.Where(h => h.Hoadonnuoc.KhachhangID == KhID && h.Hoadonnuoc.NamHoaDon == nam).OrderBy(h => h.Hoadonnuoc.ThangHoaDon).ToList();

            List<object> ngaychot = new List<object>();
            foreach (var chot in a)
            {
                var tuyen = chot.Hoadonnuoc.Khachhang.TuyenKHID;
                var thang = chot.Hoadonnuoc.ThangHoaDon;
                //db.TuyenDaChot.Where(t=>t.tuyen = a.Hoadonnuoc.Khachhang.TuyenkhID).OrderBy(t=>t.thang);
            }
            var kh = db.Khachhangs.Where(h => h.KhachhangID == KhID).FirstOrDefault();
            ViewBag.ngaychotList = ngaychot;
            ViewBag.ten = kh.Ten;
            ViewBag.tuyen = kh.Tuyenkhachhang;
            ViewBag.mkh = KhID;
            ViewBag.diachi = kh.Diachi;
            ViewBag.khID = kh.KhachhangID;
            return View(a);
        }
        //Báo cáo doanh thu theo tháng
        public ActionResult BaoCaoDoanhThuTheoThang()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString().Length > 0)
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["month"].ToString().Length > 0)
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }
            var prevMonth = month-1;
            var prevYear = year;
            if (month == 1)
            {
                prevMonth = 12;
                prevYear = year - 1;
            }
            var DuCoDauKy = 0;
            var NoDauKy = 0;
            //var NoDauKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == prevMonth && h.NamHoaDon == prevYear).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            //var DuCoDauKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == prevMonth && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == prevYear).Sum(h => h.SoTienDu);
            var NoCuoiKy = db.Hoadonnuocs.Where(h => h.Trangthaithu == false && h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            var DuCoCuoiKy = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year).Sum(h => h.SoTienDu);
            var HoaDonInTrongThang = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year).Sum(h => h.SoTienNopTheoThang.SoTienPhaiNop);
            var DoanhThuPhaiThu = HoaDonInTrongThang+NoDauKy-DuCoDauKy;
            var DoanhThuThang = db.Hoadonnuocs.Where(h => h.ThangHoaDon == month && h.NamHoaDon == year && h.Trangthaithu == true).Sum(h => h.SoTienNopTheoThang.SoTienDaThu);

            if (System.IO.File.ReadAllText(Server.MapPath(@"~/Controllers/doanhthu.txt"), System.Text.Encoding.Unicode).Length == 0)
            {
                DuCoDauKy = 0;
                NoDauKy = 0;
            }
            else
            {
                String[] f = null;
                var lines = System.IO.File.ReadAllLines(Server.MapPath(@"~/Controllers/doanhthu.txt"));
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    var br = lines[i].Split(' ');
                    if (br[0] == prevMonth.ToString() && br[1] == prevYear.ToString())
                    {
                        f = br;
                        break;
                    }
                }
                if (f == null)
                {
                    NoDauKy = 0;
                    DuCoDauKy = 0;
                }
                else
                {
                    NoDauKy = Convert.ToInt32(f[2]);
                    DuCoDauKy = Convert.ToInt32(f[3]);
                }
            }
          
            List<Object> doanhthu = new List<Object>();
            doanhthu.Add(NoDauKy);
            doanhthu.Add(DuCoDauKy);
            doanhthu.Add(DoanhThuPhaiThu);
            doanhthu.Add(DoanhThuThang);
            doanhthu.Add(NoCuoiKy);
            doanhthu.Add(DuCoCuoiKy);
            doanhthu.Add(month);
            doanhthu.Add(year);
            doanhthu.Add(HoaDonInTrongThang);
            ViewBag.DoanhThu = doanhthu;
         
            return View();
        }
      
        public ExcelPackage createfile(String name)
        {
            String filename="";
            if (name == "lichsu")
                filename = "Lịch sử sử dụng nước của khách hàng";
            else 
                filename = "Báo cáo doanh thu tháng";
            FileInfo newFile = new FileInfo(@"C:\"+filename+".xls");
            System.IO.File.Delete(@"C:\" + filename + ".xls");
            ExcelPackage ep = new ExcelPackage(newFile);
            ep.Workbook.Properties.Author = "NCK";
            // Tạo title cho file Excel
            ep.Workbook.Properties.Title = "Bao cao thu ngan";
            // Add Sheet vào file Excel
            ep.Workbook.Worksheets.Add(filename);
            var workSheet = ep.Workbook.Worksheets[1];
            return ep;
        }

        [HttpPost]
        public void Report(String KHID, String name, String nam)
        {

            var excelPackage = createfile(name);
            var KhID = Convert.ToInt32(KHID);
            var year = Convert.ToInt32(nam);
            var a = db.Chitiethoadonnuocs.Where(h => h.Hoadonnuoc.KhachhangID == KhID && h.Hoadonnuoc.NamHoaDon == year).OrderBy(h => h.Hoadonnuoc.ThangHoaDon).ToList();
            var workSheet = excelPackage.Workbook.Worksheets[1];
            workSheet.Cells[6, 1].Value = "Tháng";
            workSheet.Cells[6, 2].Value = "Ngày Chốt";
            workSheet.Cells[6, 3].Value = "CS cũ";
            workSheet.Cells[6, 4].Value = "CS mới";
            workSheet.Cells[6, 5].Value = "SH1";
            workSheet.Cells[6, 6].Value = "SH2";
            workSheet.Cells[6, 7].Value = "SH3";
            workSheet.Cells[6, 8].Value = "SH4";
            workSheet.Cells[6, 9].Value = "HCCC";
            workSheet.Cells[6, 10].Value = "SXVC";
            workSheet.Cells[6, 11].Value = "KD";
            workSheet.Cells[6, 12].Value = "Tổng sản lượng";
            workSheet.Cells[6, 13].Value = "Thành tiền";
            int i = 1;
            foreach (var item in a)
            {
                workSheet.Cells[i + 6, 1].Value = item.Hoadonnuoc.ThangHoaDon+"/"+item.Hoadonnuoc.NamHoaDon;
               // workSheet.Cells[i + 6, 2].Value = ;
                workSheet.Cells[i + 6, 3].Value = item.Chisocu;
                workSheet.Cells[i + 6, 4].Value = item.Chisomoi;
                workSheet.Cells[i + 6, 5].Value = item.SH1;
                workSheet.Cells[i + 6, 6].Value = item.SH2;
                workSheet.Cells[i + 6, 7].Value = item.SH3;
                workSheet.Cells[i + 6, 8].Value = item.SH4;
                workSheet.Cells[i + 6, 9].Value = item.HC;
                workSheet.Cells[i + 6, 10].Value = item.SXXD;
                workSheet.Cells[i + 6, 11].Value = item.KDDV;
                if (item.Chisomoi != null)
                {
                    workSheet.Cells[i + 6, 12].Value = item.Chisocu.Value - item.Chisomoi.Value;
                }
                else
                    workSheet.Cells[i + 6, 12].Value = 0;
                if (item.Hoadonnuoc.SoTienNopTheoThang != null)
                    workSheet.Cells[i + 6, 13].Value = item.Hoadonnuoc.SoTienNopTheoThang.SoTienPhaiNop;
                else
                    workSheet.Cells[i + 6, 13].Value = 0;
                i++;
            }
            int row = i + 6;
            for (int j = 6; j <= row; j++)
            {
                for (int g = 1; g <= 13; g++)
                {
                    workSheet.Cells[j, g].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }
            excelPackage.Save();
            String dir = excelPackage.File.DirectoryName.ToString() + "/" + excelPackage.File.Name;
            ViewBag.dir = dir;
            System.Diagnostics.Process.Start(dir);

        }

        //Bảng kê khách hàng dư nợ theo tháng
        public ActionResult BangKeKhachHangDuNoThang()
        {
            return View();
        }
        //Danh sách khách hàng in hóa đơn theo tháng
        public ActionResult DanhSachKhachHangInHoaDonTheoThang()
        {
            return View();
        }
        //Danh sách khách hàng không thu phí bảo vệ môi trường
        public ActionResult DanhSachKhachHangKhongThuPhiBVMT()
        {
            return View();
        }
	}
}