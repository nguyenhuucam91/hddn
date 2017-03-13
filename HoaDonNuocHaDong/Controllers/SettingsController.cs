using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class SettingsController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        // GET: Settings
        public ActionResult Backup()
        {
            string dbPath = Server.MapPath("~/App_Data/DBBackup.bak");

            var cmd = String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, MEDIANAME='DbBackups', MEDIADESCRIPTION='Media set for {0} database';"
                , "HoaDonHaDong", dbPath);
            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);

            return new FilePathResult(dbPath, "application/octet-stream");
        }


        [HttpPost]
        public ActionResult Restore(HttpPostedFileBase file)
        {
            //upload file vào thư mục App_Data
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/App_Data"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    string dbPath = Server.MapPath("~/App_Data/"+file.FileName);
                    var cmd = String.Format("USE master restore DATABASE {0} from DISK='{1}' WITH MOVE {2} TO {3}, MOVE {4} TO {5}, REPLACE;", "HoaDonHaDong1", dbPath
                        ,"C:/Program Files/Microsoft SQL Server/MSSQL11.SQLEXPRESS/MSSQL/DATA/HoaDonHaDong.mdf",
                        "~/App_Data/HoaDonHaDong.mdf",
                        "C:/Program Files/Microsoft SQL Server/MSSQL11.SQLEXPRESS/MSSQL/DATA/HoaDonHaDong_log.ldf",
                        "~/App_Data/HoaDonHaDong_log.ldf");
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }



    }
}