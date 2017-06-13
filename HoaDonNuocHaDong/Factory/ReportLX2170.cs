﻿using CrystalDecisions.CrystalReports.Engine;
using HoaDonNuocHaDong.Controllers;
using HoaDonNuocHaDong.Factory.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Factory
{
    class ReportLX2170 : IReportInHoaDon
    {
        Reports.Report report = new Reports.Report();

        public ReportLX2170()
        {
            report.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/Report.rpt")));            
        }



        public override Stream generateReportPrintAllPreview(int TuyenID, int month, int year)
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

        public override Stream generateReportPrintAll(int TuyenID, int month, int year)
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

        public override Stream generateReportPrintSelectedPreview(String[] selectedFrom, int TuyenID, int month, int year)
        {
            List<dynamic> ls = new List<dynamic>();
            foreach (var item in selectedFrom)
            {
                int HoaDonID = Convert.ToInt32(item);
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year && p.HoaDonID == HoaDonID
                              select new
                              {
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
                              }).FirstOrDefault();
                ls.Add(source);
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

        public override Stream generateReportPrintSelected(string[] selectedFrom, int TuyenID, int month, int year)
        {
            List<dynamic> ls = new List<dynamic>();
            foreach (var item in selectedFrom)
            {
                int HoaDonID = Convert.ToInt32(item);
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year && p.HoaDonID == HoaDonID
                              select new
                              {
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
                              }).FirstOrDefault();
                ls.Add(source);
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

        public override Stream generateReportPrintFromToPreview(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH)
        {
            List<dynamic> ls = new List<dynamic>();
            for (int i = fromSoHoaDon; i <= toSoHoaDon; i++)
            {
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year
                                  && p.TTThungan == (p.TTDoc + "/" + tuyenKH.Matuyen + " - " + i)
                              select new
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
                              }).FirstOrDefault();
                ls.Add(source);
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

        public override Stream generateReportPrintFromTo(int fromSoHoaDon, int toSoHoaDon, int TuyenID, int month, int year, Tuyenkhachhang tuyenKH)
        {
            List<dynamic> ls = new List<dynamic>();
            for (int i = fromSoHoaDon; i <= toSoHoaDon; i++)
            {
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year
                                  && p.TTThungan == (p.TTDoc + "/" + tuyenKH.Matuyen + " - " + i)
                              select new
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
                              }).FirstOrDefault();
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