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

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(subPath);
            }

            String pathBuilder = subPath + dbFileName + ".bak";
            string dbPath = Server.MapPath(pathBuilder);

            backupRepository.executeBackupTransaction(dbPath, dbFileName);
            backupRepository.updateOrCreateBackupRecord(LoggedInUser.NguoidungID);

            ViewBag.SuccessMessage = "Sao lưu cơ sở dữ liệu thành công";
            return View();
        }
    }
}