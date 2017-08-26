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
    class ReportTallyGenicom : IReportInHoaDon
    {
        public ReportTallyGenicom()
            : base()
        {

        }
    

        public override ReportClass setReportClass()
        {
            return new Reports.TallyGenicom();
        }

        public override string setPath()
        {
            return Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/TallyGenicom.rpt"));
        }

    }
}