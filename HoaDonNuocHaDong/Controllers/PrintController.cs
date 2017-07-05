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
using PagedList;
using HvitFramework;
using HoaDonNuocHaDong.Models.SoLieuTieuThu;
using HoaDonNuocHaDong.Models.TuyenKhachHang;

namespace HoaDonNuocHaDong.Controllers
{
    public class PrintController : BaseController
    {
        private ChiSo cS = new ChiSo();
        private HoaDonNuocHaDong.Helper.Tuyen _tuyen = new HoaDonNuocHaDong.Helper.Tuyen();
        public static string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
        private int printCircumstance = 0;
        QuanHuyenHelper qHHelper = new QuanHuyenHelper();
        Lichsuhoadon lichSuHoaDon = new Lichsuhoadon();
        TuyenKhachHangDuocChot tuyenDuocChot = new TuyenKhachHangDuocChot();



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
            ControllerBase<Models.InHoaDon.TuyenTinhTien> cb = new ControllerBase<Models.InHoaDon.TuyenTinhTien>();
            List<Models.InHoaDon.TuyenTinhTien> hoaDons = cb.Query("DanhSachHoaDonDuocInTheoTuyenThangNam",
                       new SqlParameter("@d1", thangIn),
                       new SqlParameter("@d2", namIn),
                       new SqlParameter("@d3", tuyenID)
                       );

            return hoaDons;
        }

        public void updateSoHoaDonBasedOnSituation(int? quan, String tuyenID, int thangIn, int namIn, String[] hoaDons = null, int fromReceipt = 0, int toReceipt = 0)
        {
            xoaThongTinThuNganVaCongDon(tuyenID.ToString(), thangIn.ToString(), namIn.ToString());
            switch (printCircumstance)
            {
                case (int)PrintModeEnum.PRINT_ALL:
                    updateAllHoaDon(quan, tuyenID, thangIn, namIn);
                    break;
                case (int)PrintModeEnum.PRINT_SELECTED:
                    updateSelectedReceipt(tuyenID, thangIn, namIn, hoaDons);
                    break;
                case (int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT:
                    updateFromReceiptToReceipt(quan, tuyenID, thangIn, namIn, fromReceipt, toReceipt);
                    break;
            }
        }

        private void updateFromReceiptToReceipt(int? quan, string tuyenID, int thangIn, int namIn, int fromReceipt, int toReceipt)
        {
            var hoadons = getDanhSachHoaDonDuocIn(tuyenID, thangIn, namIn);
            int soHoaDon = 1;
            double tongTienCongDon = 0;
            double truocThue = 0; double thueVAT = 0; double phiBVMT = 0; double soTienHoaDon = 0;
            String ttVoOng = qHHelper.getTTVoOng(quan);
            using (SqlConnection connection = new SqlConnection(connectionString))

                foreach (var hoadon in hoadons)
                {
                    truocThue = Math.Floor((hoadon.SH1 * hoadon.SH1Price) + (hoadon.SH2 * hoadon.SH2Price) + (hoadon.SH3 * hoadon.SH3Price) + (hoadon.SH4 * hoadon.SH4Price)
                     + (hoadon.CC * hoadon.CCPrice) + (hoadon.HC * hoadon.HCPrice) + (hoadon.SX * hoadon.SXPrice) + (hoadon.KD * hoadon.KDPrice));
                    thueVAT = Math.Round(truocThue * 0.05, 0);
                    phiBVMT = Math.Round(truocThue * (hoadon.TileBVMT / 100), 0);
                    soTienHoaDon = truocThue + thueVAT + phiBVMT;
                    var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);

                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        connection.Open();
                        command.CommandText = "Update Lichsuhoadon set TTVoOng=@ttVoOng, TTThungan = @TTThuNgan, TruocThue=@truocThue, ThueSuatPrice=@thueVAT, PhiBVMT=@phi, TongCong=@tong, BangChu=@chu, ChiSoCongDon=@chiSo " +
                            "WHERE HoaDonID = @HoaDonID";
                    
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
                        command.Parameters.AddWithValue("@truocThue", truocThue);
                        command.Parameters.AddWithValue("@HoaDonID", hoadon.HoaDonNuoc);
                        command.Parameters.AddWithValue("@thueVAT", thueVAT);
                        command.Parameters.AddWithValue("@phi", phiBVMT);
                        command.Parameters.AddWithValue("@tong", soTienHoaDon);
                        command.Parameters.AddWithValue("@chu", ConvertMoney.So_chu(soTienHoaDon));
                        command.Parameters.AddWithValue("@ttVoOng", ttVoOng);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    soHoaDon++;
                }
        }

        private void updateSelectedReceipt(string tuyenID, int thangIn, int namIn, String[] hoaDons)
        {
            int soHoaDon = 1; double tongTienCongDon = 0;
            double truocThue = 0; double thueVAT = 0; double phiBVMT = 0; double soTienHoaDon = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

                foreach (var hoaDon in hoaDons)
                {
                    int hoaDonID = Convert.ToInt32(hoaDon);
                    Lichsuhoadon hoadon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hoaDonID);
                    if (hoadon != null)
                    {
                        tongTienCongDon += hoadon.TongCong.Value;
                        truocThue = Math.Floor((hoadon.SH1.Value * hoadon.SH1Price.Value) + (hoadon.SH2.Value * hoadon.SH2Price.Value) + (hoadon.SH3.Value * hoadon.SH3Price.Value) + (hoadon.SH4.Value * hoadon.SH4Price.Value)
                       + (hoadon.CC.Value * hoadon.CCPrice.Value) + (hoadon.HC.Value * hoadon.HCPrice.Value) + (hoadon.SX.Value * hoadon.SXPrice.Value) + (hoadon.KD.Value * hoadon.KDPrice.Value));
                        thueVAT = Math.Round(truocThue * 0.05, 0);
                        phiBVMT = Math.Round(truocThue * (hoadon.TileBVMT.Value / 100), 0);
                        soTienHoaDon = truocThue + thueVAT + phiBVMT;
                        var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);
                        using (SqlCommand command = new SqlCommand("", connection))
                        {
                            connection.Open();
                            command.CommandText = "Update Lichsuhoadon set TTThungan = @TTThuNgan, TruocThue=@truocThue, ThueSuatPrice=@thueVAT, PhiBVMT=@phi, TongCong=@tong, BangChu=@chu, ChiSoCongDon=@chiSo " +
                            "WHERE HoaDonID = @HoaDonID";
                            command.Parameters.AddWithValue("@TTThuNgan", hoadon.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon);
                            command.Parameters.AddWithValue("@truocThue", truocThue);
                            command.Parameters.AddWithValue("@thueVAT", thueVAT);
                            command.Parameters.AddWithValue("@phi", phiBVMT);
                            command.Parameters.AddWithValue("@tong", soTienHoaDon);
                            command.Parameters.AddWithValue("@chu", ConvertMoney.So_chu(soTienHoaDon));
                            command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                            command.Parameters.AddWithValue("@HoaDonID", hoaDonID);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        soHoaDon++;
                    }
                }
        }

        private List<TuyenTinhTien> updateAllHoaDon(int? quanHuyenID, String tuyenID, int thangIn, int namIn)
        {
            List<TuyenTinhTien> hoadons = getDanhSachHoaDonDuocIn(tuyenID, thangIn, namIn);
            int soHoaDon = 1;
            double tongTienCongDon = 0;
            double truocThue = 0; double thueVAT = 0; double phiBVMT = 0; double soTienHoaDon = 0;
            String ttVoOng = qHHelper.getTTVoOng(quanHuyenID);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var hoadon in hoadons)
                {
                    tongTienCongDon += hoadon.TongCong;
                    truocThue = Math.Floor((hoadon.SH1 * hoadon.SH1Price) + (hoadon.SH2 * hoadon.SH2Price) + (hoadon.SH3 * hoadon.SH3Price) + (hoadon.SH4 * hoadon.SH4Price)
                        + (hoadon.CC * hoadon.CCPrice) + (hoadon.HC * hoadon.HCPrice) + (hoadon.SX * hoadon.SXPrice) + (hoadon.KD * hoadon.KDPrice));
                    thueVAT = Math.Round(truocThue * 0.05, 0);
                    phiBVMT = Math.Round(truocThue * (hoadon.TileBVMT / 100), 0);
                    soTienHoaDon = truocThue + thueVAT + phiBVMT;
                    var tuyenKH = db.Tuyenkhachhangs.Find(hoadon.TuyenKHID);
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        command.CommandText = "Update Lichsuhoadon set TTVoOng = @ttVoOng, TTThungan = @TTThuNgan, TruocThue=@truocThue, ThueSuatPrice=@thueVAT, PhiBVMT=@phi,TongCong=@tong,BangChu=@chu, ChiSoCongDon=@chiSo " +
                            "WHERE HoaDonID = @HoaDonID";
                        command.Parameters.AddWithValue("@ttVoOng", ttVoOng);
                        command.Parameters.AddWithValue("@TTThuNgan", hoadon.TTDoc + "/" + tuyenKH.Matuyen + " - " + soHoaDon);
                        command.Parameters.AddWithValue("@truocThue", truocThue);
                        command.Parameters.AddWithValue("@thueVAT", thueVAT);
                        command.Parameters.AddWithValue("@phi", phiBVMT);
                        command.Parameters.AddWithValue("@tong", soTienHoaDon);
                        command.Parameters.AddWithValue("@chu", ConvertMoney.So_chu(soTienHoaDon));
                        command.Parameters.AddWithValue("@chiSo", tongTienCongDon);
                        command.Parameters.AddWithValue("@HoaDonID", hoadon.HoaDonNuoc);
                        command.ExecuteNonQuery();
                    }
                    soHoaDon++;
                }
                connection.Close();
            }
            return hoadons;
        }

        [HttpPost]
        public ActionResult PrintPreviewSelected(FormCollection form, int? quan, int TuyenID, int month, int year)
        {
            String[] selectedReceipt = form["printSelectedHidden"].Split(',');
            String[] selectedForm = LichSuHoaDon.sortLichSuHoaDonByTTDoc(selectedReceipt);
            setPrintCircumstance((int)PrintModeEnum.PRINT_SELECTED);
            updateSoHoaDonBasedOnSituation(quan, TuyenID.ToString(), month, year, selectedForm);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintSelectedPreview(selectedForm, TuyenID, month, year);
            }
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
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
        public ActionResult PrintSelected(FormCollection form, int? quan, int TuyenID, int month, int year)
        {

            String[] selectedReceipt = form["printSelectedHidden"].Split(',');
            String[] selectedForm = LichSuHoaDon.sortLichSuHoaDonByTTDoc(selectedReceipt);
            setPrintCircumstance((int)PrintModeEnum.PRINT_SELECTED);
            updateSoHoaDonBasedOnSituation(quan, TuyenID.ToString(), month, year, selectedForm);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintSelected(selectedForm, TuyenID, month, year);
            }
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
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
                if (DateTime.Now > new DateTime(hoaDonObj.NamHoaDon.Value, hoaDonObj.ThangHoaDon.Value, 28))
                {
                    hoaDonObj.NgayIn = new DateTime(hoaDonObj.NamHoaDon.Value, hoaDonObj.ThangHoaDon.Value, 28);
                }
                else
                {
                    hoaDonObj.NgayIn = DateTime.Now;
                }
                db.Entry(hoaDonObj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public List<LichSuHoaDon> GetDanhSachHoaDons(int TuyenID, int month, int year)
        {
            ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> cb = new ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon>();
            List<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> hoaDons = cb.Query("LichSuHoaDonStoredProc",
                       new SqlParameter("@d1", month),
                       new SqlParameter("@d2", year),
                       new SqlParameter("@d3", TuyenID)
                       );

            return hoaDons;
        }

        [HttpPost]
        public ActionResult PrintPreviewFrom(FormCollection form, int? quan, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT);
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(TuyenID);
            int fromSoHoaDon = String.IsNullOrEmpty(form["from"]) ? 1 : Convert.ToInt16(form["from"]);
            int toSoHoaDon = String.IsNullOrEmpty(form["to"]) ? count : Convert.ToInt16(form["to"]);
            updateFromReceiptToReceipt(quan, TuyenID.ToString(), month, year, fromSoHoaDon, toSoHoaDon);

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
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
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
        public ActionResult PrintFrom(String printFrom, FormCollection form, int? quan, int TuyenID, int month, int year)
        {
            switch (printFrom)
            {
                case "Xem trước in danh sách theo số hóa đơn":
                    return PrintPreviewFrom(form, quan, TuyenID, month, year);
                case "In danh sách theo số hóa đơn":
                    return PrintFromTo(form, quan, TuyenID, month, year);
            }
            return View();
        }

        public ActionResult PrintFromTo(FormCollection form, int? quan, int TuyenID, int month, int year)
        {            
            setPrintCircumstance((int)PrintModeEnum.PRINT_FROM_RECEIPT_TO_RECEIPT);
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year);
            Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(TuyenID);
            int fromSoHoaDon = String.IsNullOrEmpty(form["from"]) ? 1 : Convert.ToInt16(form["from"]);
            int toSoHoaDon = String.IsNullOrEmpty(form["to"]) ? count : Convert.ToInt16(form["to"]);
            updateFromReceiptToReceipt(quan, TuyenID.ToString(), month, year, fromSoHoaDon, toSoHoaDon);

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
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
                str = report.generateReportPrintFromTo(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintFromTo(fromSoHoaDon, toSoHoaDon, TuyenID, month, year, tuyenKH);
            }

            return File(str, "application/pdf");
        }

        public ActionResult PrintAllPreview(FormCollection form, int? quan, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_ALL);            
            updateAllHoaDon(quan, TuyenID.ToString(), month, year);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintAllPreview(TuyenID, month, year);
            }
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
                str = report.generateReportPrintAllPreview(TuyenID, month, year);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintAllPreview(TuyenID, month, year);
            }

            return File(str, "application/pdf");
        }

        public ActionResult printAll(FormCollection form, int? quan, int TuyenID, int month, int year)
        {
            setPrintCircumstance((int)PrintModeEnum.PRINT_ALL);            
            updateAllHoaDon(quan, TuyenID.ToString(), month, year);
            String formPrintMachine = form["printMachine"];
            Stream str = null;
            if (formPrintMachine == "LQ2190")
            {
                Factory.ReportLP2190 report = new Factory.ReportLP2190();
                str = report.generateReportPrintAll(TuyenID, month, year);
            }
            else if (formPrintMachine == "LX2170")
            {
                Factory.ReportLX2170 report = new Factory.ReportLX2170();
                str = report.generateReportPrintAll(TuyenID, month, year);
            }
            else
            {
                Factory.ReportTallyGenicom report = new Factory.ReportTallyGenicom();
                str = report.generateReportPrintAll(TuyenID, month, year);
            }
            return File(str, "application/pdf");
        }

        public ActionResult ChiSoTuyen(int? quan, int? to, int? thang, int? nam, int page = 1)
        {
            int thangDuocChon = thang == null ? DateTime.Now.Month : thang.Value;
            int namDuocChon = nam == null ? DateTime.Now.Year : nam.Value;
            int soLuongQuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false).ToList().Count();


            IEnumerable<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot> tuyens = new List<HoaDonNuocHaDong.Models.TuyenKhachHang.TuyenKhachHangDuocChot>();
            if (soLuongQuanHuyen > 0)
            {
                Quanhuyen quanHuyenDauTien = db.Quanhuyens.FirstOrDefault(p => p.IsDelete == false);
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH && p.QuanHuyenID == quanHuyenDauTien.QuanhuyenID).ToList();
            }
            else
            {
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
            }
            bool loggedInUserIsThuNgan = HDNHD.Core.Models.RequestScope.UserRole == EUserRole.ThuNgan ? true : false;
            int phongBanId = getPhongBanNguoiDung();

            if (thang != null)
            {
                tuyens = _tuyen.getDanhSachTuyensDuocChot(quan, to, null, thangDuocChon, namDuocChon);
            }


            #region ViewBag
            ViewBag.isThuNgan = loggedInUserIsThuNgan;
            ViewData["nhanviens"] = new List<Nhanvien>();
            ViewBag.selectedQuan = quan;
            ViewBag.selectedTo = to;
            ViewBag.selectedMonth = thangDuocChon;
            ViewBag.selectedYear = namDuocChon;
            ViewBag.selectedNhanvien = 0;
            ViewBag.beforeFiltered = false;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";

            ViewData["xinghiep"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();

            #endregion

            int pageSize = (int)EPaginator.PAGESIZE;
            int pageNumber = page != 0 ? page : 0;
            ViewBag.currentPage = page;
            ViewBag.pageSize = pageSize;
            return View(tuyens);
        }


        [HttpPost]
        public ActionResult ChiSoTuyen(FormCollection form, int? quan, int? to, int? nhanvien, int? thang, int? nam, int page = 1)
        {
            String tuKhoaTimKiem = form["tukhoa"];
            int soLuongQuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false).ToList().Count();

            IEnumerable<TuyenKhachHangDuocChot> tuyens = new List<TuyenKhachHangDuocChot>();
            bool loggedInUserIsThuNgan = HDNHD.Core.Models.RequestScope.UserRole == EUserRole.ThuNgan ? true : false;
            if (!loggedInUserIsThuNgan)
            {
                if (soLuongQuanHuyen > 0)
                {
                    ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH && p.QuanHuyenID == quan).ToList();
                }
                else
                {
                    ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH).ToList();
                }
            }
            else
            {
                ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.THUNGAN).ToList();
            }

            if (thang != null)
            {
                tuyens = _tuyen.getDanhSachTuyensDuocChot(quan, to, null, thang, nam);
            }

            if (!String.IsNullOrEmpty(tuKhoaTimKiem))
            {
                tuyens = tuyens.Where(p => p.MaTuyenKH == tuKhoaTimKiem || p.TenTuyen.Contains(tuKhoaTimKiem));
            }

            int phongBanId = getPhongBanNguoiDung();

            #region ViewBag
            ViewBag.beforeFiltered = false;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewBag.isThuNgan = loggedInUserIsThuNgan;               
            ViewBag.selectedTo = to;
            ViewBag.selectedMonth = thang;
            ViewBag.selectedYear = nam;
            ViewBag.selectedNhanvien = nhanvien;
            ViewData["nhanviens"] = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId && p.ToQuanHuyenID == to).ToList();
            ViewData["xinghiep"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();

            int pageSize = (int)EPaginator.PAGESIZE;
            int pageNumber = page != 0 ? page : 0;
            ViewBag.currentPage = page;
            ViewBag.pageSize = pageSize;
            #endregion
            return View(tuyens.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult XemChiTiet(int? quan, String tuyen, String month, String year)
        {
            //Cập nhật trạng thái tính tiền
            int tuyenInt = Convert.ToInt32(tuyen);
            int monthInt = Convert.ToInt32(month);
            int yearInt = Convert.ToInt32(year);
            List<TuyenTinhTien> hoadons = getDanhSachHoaDonDuocIn(tuyen, monthInt, yearInt);
            TuyenDuocChot chotTuyen = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenInt && p.Thang == monthInt && p.Nam == yearInt);
            if (chotTuyen != null)
            {
                chotTuyen.TrangThaiTinhTien = true;
                db.Entry(chotTuyen).State = EntityState.Modified;
                db.SaveChanges();
            }
            #region ViewBag
            ViewBag.dsachKH = hoadons;
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