using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HDNHD.Models.Constants;
using System.IO;
using System.Web.Mvc;
using Ionic.Zip;
using HoaDonNuocHaDong.Config;

namespace HoaDonNuocHaDong.Repositories
{
    public class BackupRepository
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        private Config.DatabaseConfig databaseConfig = new Config.DatabaseConfig();        

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

        public void executeBackupTransaction(String fileName)
        {
            String databaseIntialCatalog = databaseConfig.getCurrentDatabaseInitialCatalog();
            String dbPath = getDbBackupPath();
            if (!File.Exists(dbPath))
            {
                var cmd = String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, MEDIANAME='{2}', MEDIADESCRIPTION='Media set for {0} database';"
                    , databaseIntialCatalog, dbPath, fileName);
                db.Database.CommandTimeout = 0;
                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
            }
        }

        public Backup checkBackupRecordCurrentDate()
        {
            DateTime currentDate = DateTime.Now.Date;
            Backup hasBackupOrNull = db.Backups.FirstOrDefault(p => p.backup_date == currentDate);
            return hasBackupOrNull;
        }

        public void updateOrCreateBackupRecord(int nguoiDungId)
        {
            Backup hasBackupOrNull = checkBackupRecordCurrentDate();
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

        public void deleteOldBackupFiles()
        {
            deleteBakFile();
            int backupNumber = db.Backups.Count();
            if (backupNumber > (int)EBackupThreshold.MAX_ALLOWED_FILES)
            {
                Backup oldestFile = db.Backups.OrderBy(p => p.backup_date).FirstOrDefault();
                if (oldestFile != null)
                {
                    try
                    {
                        String subPath = DatabaseConfig.getSubPath();
                        string pathName = HttpContext.Current.Server.MapPath(subPath + oldestFile.backup_filename + ".zip");
                        if (File.Exists(pathName))
                        {
                            File.Delete(pathName);
                            deleteDatabaseRecordInDb(oldestFile.backup_filename);
                        }
                    }
                    catch (Exception e)
                    {
                        String exception = e.ToString();
                    }
                }

            }
        }

        private void deleteBakFile()
        {
            string pathName = getDbBackupPath();
            try
            {
                File.Delete(pathName);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        private void deleteDatabaseRecordInDb(String backupFileName)
        {
            Backup backup = db.Backups.FirstOrDefault(p => p.backup_filename == backupFileName);
            if (backup != null)
            {
                db.Backups.Remove(backup);
                db.SaveChanges();
            }
        }

        internal string getDbBackupPath()
        {
            String dbFileName = setupBackupFileName();
            String subPath = setSubPath();
            String pathBuilder = subPath + dbFileName + ".bak";
            string dbPath = HttpContext.Current.Server.MapPath(pathBuilder);
            return dbPath;
        }

        internal string setSubPath()
        {
            String subPath = DatabaseConfig.getSubPath();
            return subPath;
        }

        public void applyBackupProcess(int nguoiDungId)
        {
            String dbFileName = setupBackupFileName();
            String dbPath = getDbBackupPath();
            executeBackupTransaction(dbFileName);
            zipBackupFile(dbFileName, dbPath);
            updateOrCreateBackupRecord(nguoiDungId);
            deleteOldBackupFiles();            
        }

        private void zipBackupFile(String fileName, String dbPath)
        {
            //ZipFile zip = new ZipFile();
            //zip.AddFile(dbPath);
            //String subPath = setSubPath();
            //String pathBuilder = subPath + fileName + ".zip";
            //String destZipFile = HttpContext.Current.Server.MapPath(pathBuilder);
            //zip.Save(destZipFile);
        }
    }
}