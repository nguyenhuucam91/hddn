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
            backupRepository.applyBackupProcess(LoggedInUser.NguoidungID);
            return View();
        }
    }
}