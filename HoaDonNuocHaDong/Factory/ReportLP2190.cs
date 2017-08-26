using HoaDonNuocHaDong.Factory.Interface;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;


namespace HoaDonNuocHaDong.Factory
{
    class ReportLP2190 : IReportInHoaDon
    {
        public ReportLP2190() : base()
        {
            
        }

        public override CrystalDecisions.CrystalReports.Engine.ReportClass setReportClass()
        {
            return new Reports.ReportLP2190();
        }

        public override string setPath()
        {
            return Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/ReportLP2190.rpt"));
        }                             
        
    }
}