using CrystalDecisions.CrystalReports.Engine;
using HoaDonNuocHaDong.Controllers;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Factory.Interface
{
    public abstract class IReportInHoaDon
    {
        protected PrintController printCtl = new PrintController();
        protected HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        protected ReportDocument report;
        protected String path;

        public IReportInHoaDon()
        {
            this.makeReport();
        }

        public abstract ReportClass setReportClass();
        public abstract String setPath();

        public void makeReport()
        {
            this.report = setReportClass();
            this.path = setPath();
        }

        public Stream generateReportPrintAllPreview(int TuyenID, int month, int year)
        {
            var source = printCtl.GetDanhSachHoaDons(TuyenID, month, year).ToList();
            //cập nhật trạng thái in cho tất cả các hóa đơn
            foreach (var item in source)
            {
                int hoaDonID = item.HoaDonID;
            }
            //đặt datasource để đẩy vào crystal report
            report.SetDataSource(source);
            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public Stream generateReportPrintAll(int TuyenID, int month, int year)
        {
            var source = printCtl.GetDanhSachHoaDons(TuyenID, month, year).ToList();
            //cập nhật trạng thái in cho tất cả các hóa đơn
            foreach (var item in source)
            {
                int hoaDonID = item.HoaDonID;
                printCtl.CapNhatTrangThaiIn(hoaDonID);
            }
            //đặt datasource để đẩy vào crystal report
            report.SetDataSource(source);
            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public Stream generateReportPrintSelectedPreview(String[] selectedFrom, int TuyenID, int month, int year)
        {
            List<dynamic> ls = new List<dynamic>();
            foreach (var item in selectedFrom)
            {
                int HoaDonID = Convert.ToInt32(item);
                ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> cb = new ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon>();
                HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon hoaDon = cb.Query("InHoaDonSelectedAndFromTo",
                           new SqlParameter("@d1", month),
                           new SqlParameter("@d2", year),
                           new SqlParameter("@d3", TuyenID),
                           new SqlParameter("@hoadonid", HoaDonID)
                           ).FirstOrDefault();
                ls.Add(hoaDon);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public Stream generateReportPrintSelected(String[] selectedFrom, int TuyenID, int month, int year)
        {
            List<dynamic> ls = new List<dynamic>();
            foreach (var item in selectedFrom)
            {
                int HoaDonID = Convert.ToInt32(item);
                ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> cb = new ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon>();
                HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon hoaDon = cb.Query("InHoaDonSelectedAndFromTo",
                           new SqlParameter("@d1", month),
                           new SqlParameter("@d2", year),
                           new SqlParameter("@d3", TuyenID),
                           new SqlParameter("@hoadonid", HoaDonID)
                           ).FirstOrDefault();
                ls.Add(hoaDon);
                printCtl.CapNhatTrangThaiIn(HoaDonID);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public Stream generateReportPrintFromToPreview(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH)
        {
            List<dynamic> ls = new List<dynamic>();
            for (int i = fromSoHoaDon; i <= toSoHoaDon; i++)
            {
                ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> cb = new ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon>();
                HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon hoaDon = cb.Query("InHoaDonFromTo",
                           new SqlParameter("@d1", month),
                           new SqlParameter("@d2", year),
                           new SqlParameter("@d3", TuyenID),
                           new SqlParameter("@matuyen", tuyenKH.Matuyen),
                           new SqlParameter("@num", i)
                           ).FirstOrDefault();
                ls.Add(hoaDon);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public Stream generateReportPrintFromTo(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH)
        {
            List<dynamic> ls = new List<dynamic>();
            for (int i = fromSoHoaDon; i <= toSoHoaDon; i++)
            {
                ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon> cb = new ControllerBase<HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon>();
                HoaDonNuocHaDong.Models.InHoaDon.LichSuHoaDon source = cb.Query("InHoaDonFromTo",
                           new SqlParameter("@d1", month),
                           new SqlParameter("@d2", year),
                           new SqlParameter("@d3", TuyenID),
                           new SqlParameter("@matuyen", tuyenKH.Matuyen),
                           new SqlParameter("@num", i)
                           ).FirstOrDefault();
                ls.Add(source);
                //cập nhật trạng thái in cho hóa đơn được in từ số thứ tự (số hóa đơn) từ xx->yy
                int hoaDonID = source.HoaDonID;
                //cập nhật trạng thái in vào tất cả các hóa đơn
                printCtl.CapNhatTrangThaiIn(hoaDonID);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}