using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.Admin.Helpers
{
    public static class DatabaseHelpers
    {
        public static void RestoreDatabase(String backUpFile, String serverName, String databaseName, String userName, String password)
        {
            ServerConnection connection = new ServerConnection(serverName, userName, password);
            Server server = new Server(connection);
            Restore restore = new Restore();
            restore.Action = RestoreActionType.Database;
            restore.Database = databaseName;
            BackupDeviceItem bkpDevice = new BackupDeviceItem(backUpFile, DeviceType.File);
            
            restore.Devices.Add(bkpDevice);
            restore.ReplaceDatabase = true;

            //// Kill all processes
            //server.KillAllProcesses(restore.Database);
            //// Set single-user mode
            //Database db = server.Databases[restore.Database];
            //// db.DatabaseOptions.UserAccess=true;
            //db.Alter(TerminationClause.RollbackTransactionsImmediately);
            //// Detach database
            //server.DetachDatabase(restore.Database, false);

            restore.SqlRestore(server);
        }
    }
}