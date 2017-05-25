using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
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

        public ActionResult Backup()
        {
            String dbFileName = backupRepository.setupBackupFileName();
            String subPath = "~/DBBackups/";                       

            String pathBuilder = subPath + dbFileName + ".bak";
            string dbPath = Server.MapPath(pathBuilder);

            backupRepository.executeBackupTransaction(dbPath, dbFileName);
            backupRepository.updateOrCreateBackupRecord(LoggedInUser.NguoidungID);
            
            return View();
        }
    }
}