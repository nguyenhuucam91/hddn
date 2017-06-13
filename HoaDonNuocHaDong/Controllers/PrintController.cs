using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using HoaDonNuocHaDong;
using System.Web.Routing;
using iTextSharp.text.html.simpleparser;
using System.Data.Entity;
using System.Net;
using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Reports;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Base;
using System.Configuration;
using HoaDonNuocHaDong.Models.InHoaDon;
using HDNHD.Models.Constants;
using System.Runtime.Serialization;

namespace HoaDonNuocHaDong.Controllers
{
    public class PrintController : BaseController
    {
        private ChiSo cS = new ChiSo();        
        private HoaDonNuocHaDong.Helper.Tuyen _tuyen = new HoaDonNuocHaDong.Helper.Tuyen();
        public static string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
        private int printCircumstance = 0;

        public void setPrintCircumstance(int printCircumstance)
        {
            this.printCircumstance = printCircumstance;
        }

        public void tinhTienTheoTuyen(String tuyenID, String month, String year, String chinhanhID = null, String toID = null, String nhanvien = null)
        {
            int _tuyenID = Convert.ToInt32(tuyenID);
            int _chinhanhID = Convert.ToInt32(chinhanhID);
            int _toID = Convert.ToInt32(toID);
            int _nhanVienID = Convert.ToInt32(nhanvien);

            int thangIn = String.IsNullOrEmpty(month) ? DateTime.Now.Month : Convert.ToInt32(month);
            int namIn = String.IsNullOrEmpty(year) ? DateTime.Now.Year : Convert.ToInt32(year);

            ViewBag.beforeFilter = false;

            ViewBag.chiNhanh = db.Quanhuyens.ToList();
            ViewBag.to = db.ToQuanHuyens.ToList();
            ViewBag.nhanVien = db.Nhanviens.ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.ToList();

            ViewBag.selectedChinhanhID = _chinhanhID;
            ViewBag.selectedTuyen = _tuyenID;
            ViewBag.selectedNhanVien = _nhanVienID;
            ViewBag.selectedTo = _toID;

            ViewBag.selectedMonth = thangIn;
            ViewBag.selectedYear = namIn;
        }

        public List<Models.InHoaDon.TuyenTinhTien> getDanhSachHoaDonDuocIn(String tuyenID, int thangIn, int namIn)
        {
            List<Models.InHoaDon.TuyenTinhTien> hoadons = (from i in db.Lichsuhoadons
                                                           join j in db.Hoadonnuocs on i.HoaDonID equals j.HoadonnuocID
                                                           join r in db.Khachhangs on j.KhachhangID equals r.KhachhangID
                                                           where i.ThangHoaDon == thangIn && i.NamHoaDon == namIn &&
                                                                  r.TuyenKHID.ToString() == tuyenID &&
                                                                  (j.Trangthaixoa == false || j.Trangthaixoa == null) && j.Trangthaichot == true &&
                                                                  ((r.Ngayngungcapnuoc == null && r.Ngaycapnuoclai == null) || (r.Ngaycapnuoclai.Value <= DateTime.Now)) &&
                                                                  j.Tongsotieuthu > 0
                                                           orderby i.TTDoc
                                                           select new Models.InHoaDon.TuyenTinhTien
                                                           {
                                                               HoaDonNuoc = i.HoaDonID,
                                                               MaKH = i.MaKH,
                                                               TenKH = i.TenKH,
                                                               DiaChi = i.Diachi,
                                                               NgayBatDau = i.NgayBatDau,
                                                               NgayKetThuc = i.NgayKetThuc,
                                                               SH1 = i.SH1,
                                                               SH1Price = i.SH1Price,
                                                               SH2 = i.SH2,
                                                               SH2Price = i.SH2Price,
                                                               SH3 = i.SH3,
                                                               SH3Price = i.SH3Price,
                                                               SH4 = i.SH4,
                                                               SH4Price = i.SH4Price,
                                                               HC = i.HC,
                                                               HCPrice = i.HCPrice,
                                                               CC = i.CC,
                                                               CCPrice = i.CCPrice,
                                                               SX = i.SX,
                                                               SXPrice = i.SXPrice,
                                                               KD = i.KD,
                                                               KDPrice = i.KDPrice,
                                                               PhiVAT = (i.SH1 * i.SH1Price + i.SH2 * i.SH2Price + i.SH3 * i.SH3Price + i.SH4 * i.SH4Price
                                                               + i.HC * i.HCPrice + i.CC * i.CCPrice + i.SX * i.SXPrice + i.KD * i.KDPrice) * 0.05,
                                                               PhiBVMT = (i.SH1 * i.SH1Price + i.SH2 * i.SH2Price + i.SH3 * i.SH3Price + i.SH4 * i.SH4Price
                                                               + i.HC * i.HCPrice + i.CC * i.CCPrice + i.SX * i.SXPrice + i.KD * i.KDPrice) * (i.TileBVMT/100),
                                                               TTDoc = i.TTDoc.Value,
                                                               SanLuong = i.SanLuongTieuThu,
                                                               TongCong = (i.SH1 * i.SH1Price + i.SH2 * i.SH2Price + i.SH3 * i.SH3Price + i.SH4 * i.SH4Price
                                                                          + i.HC * i.HCPrice + i.CC * i.CCPrice + i.SX * i.SXPrice + i.KD * i.KDPrice) +
                                                               
                                                                          ((i.SH1 * i.SH1Price + i.SH2 * i.SH2Price + i.SH3 * i.SH3Price + i.SH4 * i.SH4Price
                                                                          + i.HC * i.HCPrice + i.CC * i.CCPrice + i.SX * i.SXPrice + i.KD * i.KDPrice) * 0.05) +

                                                                          ((i.SH1 * i.SH1Price + i.SH2 * i.SH2Price + i.SH3 * i.SH3Price + i.SH4 * i.SH4Price
                                                               + i.HC * i.HCPrice + i.CC * i.CCPrice + i.SX * i.SXPrice + i.KD * i.KDPrice) * (i.TileBVMT / 100)),

                                                                          
                                                               TTThuNgan = i.TTThungan,
                                                               TuyenKHID = i.TuyenKHID,
                                                           }).ToList();

            return hoadons;
        }

        public void updateSoHoaDonBasedOnSituation(String tuyenID, int thangIn, int namIn, String[] hoaDons = null, int fromReceipt = 0, int toReceipt = 0)
        {
            xoaThongTinThuNganVaCongDon(tuyenID.ToString(), thangIn.ToString(), namIn.ToString());
            switch (printCircumstance)
            {
                case (int)PrintModeEnum.PRINT_ALL:
                    updateAllHoaDon(tuyenID, thangIn, namIn);
                    break;
                case (int)PrintModeEnum.PRINT_SELECTED:
                    updateSelectedReceipt(tuyenID, thangIn, namIn, hoaDons);
                    break;
                case (int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT:
                    updateFromReceiptToReceipt(tuyenID, thangIn, namIn, fromReceipt, toReceipt);
                    break;
            }
        }

        private void updateFromReceiptToReceipt(string tuyenID, int thangIn, int namIn, int fromReceipt, int toReceipt)
        {
            var hoadons = getDanhSachHoaDonDuocIn(tuyenID, thangIn, namIn);
            int soHoaDon = 1;
            double tongTienCongDon = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

                foreach (var hoadon in hoadons)
                {
                    var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        connection.Open();
                        command.CommandText = "Update Lichsuhoadon set TTThungan = @TTThuNgan,ChiSoCongDon=@chiSo WHERE HoaDonID = @HoaDonID";
                        command.Parameters.AddWithValue("@TTThuNgan", hoadon.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon);
                        if (soHoaDon >= fromReceipt && soHoaDon <= toReceipt)
                        {
                            tongTienCongDon += hoadon.TongCong;
                            command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                        }
                        command.Parameters.AddWithValue("@HoaDonID", hoadon.HoaDonNuoc);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    soHoaDon++;
                }
        }

        private void updateSelectedReceipt(string tuyenID, int thangIn, int namIn, String[] hoaDons)
        {
            int soHoaDon = 1; double tongTienCongDon = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

                foreach (var hoaDon in hoaDons)
                {
                    int hoaDonID = Convert.ToInt32(hoaDon);
                    Lichsuhoadon hoadon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hoaDonID);
                    if (hoadon != null)
                    {
                        tongTienCongDon += hoadon.TongCong;
                        var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);
                        using (SqlCommand command = new SqlCommand("", connection))
                        {
                            connection.Open();
                            command.CommandText = "Update Lichsuhoadon set TTThungan = @TTThuNgan,ChiSoCongDon=@chiSo WHERE HoaDonID = @HoaDonID";
                            command.Parameters.AddWithValue("@TTThuNgan", hoadon.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon);
                            command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                            command.Parameters.AddWithValue("@HoaDonID", hoaDonID);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        soHoaDon++;
                    }
                }
        }

        private void updateAllHoaDon(String tuyenID, int thangIn, int namIn)
        {
            var hoadons = getDanhSachHoaDonDuocIn(tuyenID, thangIn, namIn);
            int soHoaDon = 1;
            double tongTienCongDon = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

                foreach (var hoadon in hoadons)
                {
                    tongTienCongDon += hoadon.TongCong;
                    var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        connection.Open();
                        command.CommandText = "Update Lichsuhoadon set TTThungan = @TTThuNgan,ChiSoCongDon=@chiSo WHERE HoaDonID = @HoaDonID";
                        command.Parameters.AddWithValue("@TTThuNgan", hoadon.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon);
                        command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                        command.Parameters.AddWithValue("@HoaDonID", hoadon.HoaDonNuoc);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    soHoaDon++;
                }
        }

        [HttpPost]
        public ActionResult PrintPreviewSelected(FormCollection form, int TuyenID, int month, int year)
        {
            String[] selectedForm = form["printSelectedHidden"].Split(',');
            Array.Sort(selectedForm);
            setPrintCircumstance((int)PrintModeEnum.PRINT_SELECTED);
            updateSoHoaDonBasedOnSituation(TuyenID.ToString(), month, year, selectedForm);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintSelectedPreview(selectedForm, TuyenID, month, year);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintSelectedPreview(selectedForm, TuyenID, month, year);
            }
           
            return File(str, "application/pdf");
        }

        [HttpPost]
        public ActionResult PrintSelected(FormCollection form, int TuyenID, int month, int year)
        {

            String[] selectedForm = form["printSelectedHidden"].Split(',');
            Array.Sort(selectedForm);
            setPrintCircumstance((int)PrintModeEnum.PRINT_SELECTED);
            updateSoHoaDonBasedOnSituation(TuyenID.ToString(), month, year, selectedForm);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintSelected(selectedForm, TuyenID, month, year);
            }
            else 
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintSelected(selectedForm, TuyenID, month, year);
            }
           
            return File(str, "application/pdf");

        }

        public void CapNhatTrangThaiIn(int HoaDonID)
        {
            Hoadonnuoc hoaDonObj = db.Hoadonnuocs.Find(HoaDonID);
            if (hoaDonObj != null)
            {
                hoaDonObj.Trangthaiin = true;
                //thêm ngày in vào hóa đơn
                hoaDonObj.NgayIn = DateTime.Now;
                db.Entry(hoaDonObj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<LichSuHoaDon> GetDanhSachHoaDons(int TuyenID, int month, int year)
        {
            var lichsuhoadons = (from p in db.Lichsuhoadons
                                 join r in db.Hoadonnuocs on p.HoaDonID equals r.HoadonnuocID
                                 join k in db.Khachhangs on r.KhachhangID equals k.KhachhangID
                                 where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year &&
                                        (r.Trangthaixoa == false || r.Trangthaixoa == null) && r.Trangthaichot == true &&
                                        ((k.Ngayngungcapnuoc == null && k.Ngaycapnuoclai == null) || (k.Ngaycapnuoclai.Value <= DateTime.Now)) &&
                                        p.SanLuongTieuThu > 0
                                
                                 orderby p.TTDoc
                                 select new HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon
                                 {
                                     HoaDonID = p.HoaDonID,
                                     ThangHoaDon = p.ThangHoaDon,
                                     NamHoaDon = p.NamHoaDon,
                                     TenKH = p.TenKH,
                                     DiaChi = p.Diachi,
                                     MST = p.MST,
                                     MaKH = p.MaKH,
                                     SoHopDong = p.SoHopDong,
                                     SanLuongTieuThu = p.SanLuongTieuThu,
                                     ChiSoCu = p.ChiSoCu,
                                     ChiSoMoi = p.ChiSoMoi,
                                     SH1 = p.SH1,
                                     SH2 = p.SH2,
                                     SH3 = p.SH3,
                                     SH4 = p.SH4,
                                     SH1Price = p.SH1Price,
                                     SH2Price = p.SH2Price,
                                     SH3Price = p.SH3Price,
                                     SH4Price = p.SH4Price,
                                     HC = p.HC,
                                     CC = p.CC,
                                     SX = p.SX,
                                     KD = p.KD,
                                     HCPrice = p.HCPrice,
                                     CCPrice = p.CCPrice,
                                     SXPrice = p.SXPrice,
                                     KDPrice = p.KDPrice,
                                     ThueSuatPrice = p.ThueSuatPrice,
                                     TileBVMT = p.TileBVMT,
                                     PhiBVMT = p.PhiBVMT,
                                     TongCong = p.TongCong,
                                     BangChu = p.BangChu,
                                     TTVoOng = p.TTVoOng,
                                     TTThungan = p.TTThungan,
                                     NgayBatDau = p.NgayBatDau,
                                     NgayKetThuc = p.NgayKetThuc,
                                     ChiSoCongDon = p.ChiSoCongDon,
                                 });
            return lichsuhoadons.ToList();
        }

        [HttpPost]
        public ActionResult PrintPreviewFrom(FormCollection form, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT);           
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(TuyenID);
            int fromSoHoaDon = String.IsNullOrEmpty(form["from"]) ? 1 : Convert.ToInt16(form["from"]);
            int toSoHoaDon = String.IsNullOrEmpty(form["to"]) ? count : Convert.ToInt16(form["to"]);
            updateFromReceiptToReceipt(TuyenID.ToString(), month, year, fromSoHoaDon, toSoHoaDon);

            List<LichSuHoaDon> danhSachHoaDons = GetDanhSachHoaDons(TuyenID, month, year);

            if (toSoHoaDon >= danhSachHoaDons.Count)
            {
                toSoHoaDon = danhSachHoaDons.Count();
            }

            String formPrintMachine = form["printMachine"];
            Stream str = null;

            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintFromToPreview(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintFromToPreview(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }           

            return File(str, "application/pdf");
        }

        [HttpPost]
        public ActionResult PrintFrom(String printFrom, FormCollection form, int TuyenID, int month, int year)
        {
            switch (printFrom)
            {
                case "Xem trước in danh sách theo số hóa đơn":
                    return PrintPreviewFrom(form, TuyenID, month, year);
                case "In danh sách theo số hóa đơn":
                    return PrintFromTo(form, TuyenID, month, year);
            }
            return View();
        }

        public ActionResult PrintFromTo(FormCollection form, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT);            
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(TuyenID);
            int fromSoHoaDon = String.IsNullOrEmpty(form["from"]) ? 1 : Convert.ToInt16(form["from"]);
            int toSoHoaDon = String.IsNullOrEmpty(form["to"]) ? count : Convert.ToInt16(form["to"]);
            updateFromReceiptToReceipt(TuyenID.ToString(), month, year, fromSoHoaDon, toSoHoaDon);

            List<LichSuHoaDon> danhSachHoaDons = GetDanhSachHoaDons(TuyenID, month, year);

            if (toSoHoaDon >= danhSachHoaDons.Count)
            {
                toSoHoaDon = danhSachHoaDons.Count();
            }
           
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintFromTo(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintFromTo(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }           

            return File(str, "application/pdf");
        }

        public ActionResult PrintAllPreview(FormCollection form, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_ALL);
            updateSoHoaDonBasedOnSituation(TuyenID.ToString(), month, year, null);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintAllPreview(TuyenID, month, year);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintAllPreview(TuyenID, month, year);
            }          

            return File(str, "application/pdf");
        }

        public ActionResult PrintCrystalReport(FormCollection form, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_ALL);
            updateSoHoaDonBasedOnSituation(TuyenID.ToString(), month, year, null);

            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintAll(TuyenID, month, year);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintAll(TuyenID, month, year);
            }            
            return File(str, "application/pdf");
        }

        public ActionResult ChiSoTuyen()
        {
            ViewBag.beforeFiltered = true;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewData["tuyen"] = new List<Tuyenkhachhang>();
            ViewData["xinghiep"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            int soLuongQuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false).ToList().Count();
            if (soLuongQuanHuyen > 0)
            {
                Quanhuyen quanHuyenDauTien = db.Quanhuyens.FirstOrDefault(p => p.IsDelete == false);
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH && p.QuanHuyenID == quanHuyenDauTien.QuanhuyenID).ToList();
            }
            else
            {
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
            }

            #region ViewBag
            ViewBag.selectedQuan = "";
            ViewBag.selectedTo = "";
            ViewBag.selectedMonth = "";
            ViewBag.selectedYear = "";
            #endregion

            return View();
        }


        [HttpPost]
        public ActionResult ChiSoTuyen(FormCollection form)
        {
            int quanHuyen = String.IsNullOrEmpty(form["quan"]) ? 0 : Convert.ToInt32(form["quan"]);
            ViewData["xinghiep"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            //lấy danh sách tổ 
            int soLuongQuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false).ToList().Count();

            if (soLuongQuanHuyen > 0)
            {
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH && p.QuanHuyenID == quanHuyen).ToList();
            }
            else
            {
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
            }

            int to = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            //một tuyến được nhập xong chỉ số tức là tất cả hóa đơn trong đó đã nhập xong
            int month = String.IsNullOrEmpty(form["thang"]) ? DateTime.Now.Month : Convert.ToInt32(form["thang"]);
            int year = String.IsNullOrEmpty(form["nam"]) ? DateTime.Now.Year : Convert.ToInt32(form["nam"]);

            List<Tuyenkhachhang> tuyensKhachHang = new List<Tuyenkhachhang>();
            //nếu tổ ko đc chọn
            if (quanHuyen == 0 && to == 0)
            {
                //lấy toàn bộ danh sách tuyến trong hệ thống không lọc                
                List<TuyenDuocChot> tuyen = db.TuyenDuocChots.Where(p => p.Nam == year && p.Thang == month).ToList();
                foreach (var item in tuyen)
                {
                    Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(item.TuyenKHID);
                    tuyensKhachHang.Add(tuyenKH);
                }
            }
            else
            {
                //Lấy danh sách tuyến trong hệ thống lọc theo tổ
                List<int> danhSachTuyenThuocTo = _tuyen.getTuyenByTo(to).Select(p => p.TuyenCuaKH).Distinct().ToList();
                List<int> danhSachTuyenDaChot = db.TuyenDuocChots.Where(p => p.Nam == year && p.Thang == month).Select(p => p.TuyenKHID.Value).ToList();
                foreach (var r in danhSachTuyenThuocTo.Intersect(danhSachTuyenDaChot))
                {
                    Tuyenkhachhang tuyen = db.Tuyenkhachhangs.Find(r);
                    tuyensKhachHang.Add(tuyen);
                }
            }

            ViewBag.beforeFiltered = false;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";

            #region ViewBag
            ViewBag.selectedQuan = quanHuyen.ToString();
            ViewBag.selectedTo = to.ToString();
            ViewBag.selectedMonth = month;
            ViewBag.selectedYear = year;
            #endregion

            ViewData["tuyen"] = tuyensKhachHang;
            return View();
        }



        public ActionResult XemChiTiet(String tuyen, String month, String year)
        {
            int tuyenKHID = Convert.ToInt32(tuyen);
            //Cập nhật trạng thái tính tiền
            int tuyenInt = Convert.ToInt32(tuyen);
            int monthInt = Convert.ToInt32(month);
            int yearInt = Convert.ToInt32(year);
            TuyenDuocChot chotTuyen = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenInt && p.Thang == monthInt && p.Nam == yearInt);
            if (chotTuyen != null)
            {
                chotTuyen.TrangThaiTinhTien = true;
                db.Entry(chotTuyen).State = EntityState.Modified;
                db.SaveChanges();
            }
            String tuyenID = Request.QueryString["tuyen"];
            #region ViewBag
            var danhSach = getDanhSachHoaDonDuocIn(tuyenID, monthInt, yearInt);
            ViewBag.dsachKH = danhSach.OrderBy(p => p.TTDoc).ToList();
            ViewBag.selectedTuyen = tuyen;
            #endregion
            return View();
        }

        private void xoaThongTinThuNganVaCongDon(string tuyen, string month, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                connection.Open();
                command.CommandText = "Update Lichsuhoadon set TTThungan = '',ChiSoCongDon = 0 WHERE TuyenKHID = @tuyen AND ThangHoaDon = @month AND NamHoaDon = @year";
                command.Parameters.AddWithValue("@tuyen", tuyen);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }


    }
}