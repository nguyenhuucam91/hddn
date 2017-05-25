using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HDNHD.Models.Constants;

namespace HoaDonNuocHaDong.Repositories
{
    public class BackupRepository
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        private Config.DatabaseConfig databaseConfig = new Config.DatabaseConfig();
        private HDNHD.Models.DataContexts.Nguoidung LoggedInUser;

        public String setupBackupFileName()
        {
            String databaseIntialCatalog = databaseConfig.getCurrentDatabaseInitialCatalog();
            int currentDay = DateTime.Now.Day;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            String currentDate = currentDay + "" + currentMonth + "" + currentYear;
            String backupFileName = databaseIntialCatalog + "-" + currentDate;
            return backupFileName;
        }

        public void executeBackupTransaction(String dbPath, String fileName)
        {
            String databaseIntialCatalog = databaseConfig.getCurrentDatabaseInitialCatalog(); 
            var cmd = String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, MEDIANAME='{2}', MEDIADESCRIPTION='Media set for {0} database';"
                , databaseIntialCatalog, dbPath, fileName);
            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
        }

        public void updateOrCreateBackupRecord(int nguoiDungId)
        {
            DateTime currentDate = DateTime.Now.Date;
            Backup hasBackupOrNull = db.Backups.FirstOrDefault(p => p.backup_date == currentDate);
            if (hasBackupOrNull != null)
            {
                updateBackupRecord(hasBackupOrNull, nguoiDungId);
            }
            else
            {
                createBackupRecord(nguoiDungId);
            }
        }

        private void createBackupRecord(int nguoiDungId)
        {
            Backup backup = new Backup();
            backup.backup_date = DateTime.Now.Date;
            backup.backup_filename = setupBackupFileName();
            backup.user_backup_id = nguoiDungId;
            db.Backups.Add(backup);
            db.SaveChanges();
        }

        private void updateBackupRecord(Backup backedUpEntity, int nguoiDungId)
        {
            backedUpEntity.backup_date = DateTime.Now.Date;
            backedUpEntity.backup_filename = setupBackupFileName();
            backedUpEntity.user_backup_id = nguoiDungId;
            db.Entry(backedUpEntity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}