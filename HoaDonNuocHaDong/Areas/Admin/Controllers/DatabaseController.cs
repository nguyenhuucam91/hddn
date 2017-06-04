using HDNHD.Models.DataContexts;
//using HoaDonNuocHaDong.Areas.Admin.Helpers;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.Admin.Controllers
{
    public class DatabaseController : BaseController
    {
        BackupRepository backupRepository = new BackupRepository();

        public ActionResult Restore()
        {
            title = "Khôi phục dữ liệu";


            return View();
        }

        /// <summary>
        /// restore database from uploaded .bak file
        /// </summary>
        /// <requires>file is valid</requires>
        /// <effects>
        /// validate datatype
        /// get database info
        /// restore file 
        /// notify if success
        /// </effects>
        [HttpPost]
        public ActionResult Restore(HttpPostedFile file)
        {
            try
            {
                var connString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
                var connStringBuilder = new SqlConnectionStringBuilder(connString);
                var serverName = connStringBuilder.DataSource;
                var dbName = connStringBuilder.InitialCatalog;
                var username = connStringBuilder.UserID;
                var password = connStringBuilder.Password;

                //DatabaseHelpers.RestoreDatabase("HoaDonHaDong", file.FileName, )
            }
            catch (Exception e)
            {

            }

            return View();
        }

        public ActionResult Backup()
        {
            var context = uow.GetDataContext();
            backupRepository.applyBackupProcess(LoggedInUser.NguoidungID);
            return View();
        }
    }
}