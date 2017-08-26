using CrystalDecisions.CrystalReports.Engine;
using HoaDonNuocHaDong.Controllers;
using HoaDonNuocHaDong.Factory.Interface;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Factory
{
    class ReportLX2170 : IReportInHoaDon
    {
        //Reports.ReportSample report = new Reports.ReportSample();

        public ReportLX2170() : base()
        {                       
        }       

        public override ReportClass setReportClass()
        {
            return new Reports.ReportSample();
        }

        public override string setPath()
        {
            return Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/ReportSample.rpt"));
        }
    }
}