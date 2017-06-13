using HoaDonNuocHaDong.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Factory.Interface
{
    public abstract class IReportInHoaDon
    {
        protected PrintController printCtl = new PrintController();
        protected HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        public abstract Stream generateReportPrintAllPreview(int TuyenID, int month, int year);
        public abstract Stream generateReportPrintAll(int TuyenID, int month, int year);
        public abstract Stream generateReportPrintSelectedPreview(String[] selectedFrom, int TuyenID, int month, int year);
        public abstract Stream generateReportPrintSelected(String[] selectedFrom, int TuyenID, int month, int year);
        public abstract Stream generateReportPrintFromToPreview(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH);
        public abstract Stream generateReportPrintFromTo(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH);
    }
}