using System;
using System.Threading.Tasks;
using GeneralUtility.LogUtility;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace GeneralUtility
{

    public static class SqlServerBackup
    {

        /// <summary>
        /// Occurs when [backup completed].
        /// </summary>
        public static event EventHandler BackupCompleted;
        /// <summary>
        /// Occurs when [backup percentage].
        /// </summary>
        public static event EventHandler<int> BackupPercentage;
        /// <summary>
        /// Occurs when [backup error].
        /// </summary>
        public static event EventHandler<string> BackupError;

        /// <summary>
        /// The task a
        /// </summary>
        private static Task _taskA;

        /// <summary>
        /// Backups the specified database name.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="destPath">The dest path.</param>
        /// <param name="instaceName">Name of the instace.</param>
        public static void Backup(string dbName, string fileName, string destPath, string instaceName)
        {

            if (_taskA != null)
            {

                _taskA.Dispose();
                _taskA = null;

            }

            _taskA = new Task(() => BackupTask(dbName, fileName, destPath, instaceName));
            _taskA.Start();

        }


        /// <summary>
        /// Backups the task.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="destPath">The dest path.</param>
        /// <param name="instaceName">Name of the instace.</param>
        private async static void BackupTask(string dbName, string fileName, string destPath, string instaceName)
        {
            Log.NewLog(LogType.Info, "Start SQL Backup Task");
            Backup sqlBackup = new Backup();

            try
            {

                ////Specify the type of backup, the description, the name, and the database to be backed up.    
                sqlBackup.Action = BackupActionType.Database;
                sqlBackup.BackupSetDescription = "BackUp of:" + dbName + "on" + DateTime.Now.ToShortDateString();
                sqlBackup.BackupSetName = "FullBackUp";
                sqlBackup.Database = dbName;

                ////Declare a BackupDeviceItem    
                string destinationPath = destPath;
                string backupfileName = fileName + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " + DateTime.Now.Hour + DateTime.Now.Minute + ".bak";
                BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath + "\\" + backupfileName, DeviceType.File);
                ////Define Server connection    

                //ServerConnection connection = new ServerConnection(frm.serverName, frm.userName, frm.password);    
                ServerConnection connection = new ServerConnection(instaceName);

                ////To Avoid TimeOut Exception    
                Server sqlServer = new Server(connection);
                sqlServer.ConnectionContext.StatementTimeout = 60 * 60;
                Database db = sqlServer.Databases[dbName];

                sqlBackup.Initialize = true;
                sqlBackup.Checksum = true;
                sqlBackup.ContinueAfterError = true;

                ////Add the device to the Backup object.    
                sqlBackup.Devices.Add(deviceItem);
                ////Set the Incremental property to False to specify that this is a full database backup.    
                sqlBackup.Incremental = false;

                //sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);

                ////Specify that the log must be truncated after the backup is complete.    
                sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

                sqlBackup.FormatMedia = false;

                ////Subsribe Event
                sqlBackup.PercentComplete += SqlBackupOnPercentComplete;
                sqlBackup.Information += SqlBackupOnInformation;
                sqlBackup.Complete += SqlBackupOnComplete;

                ////Run SqlBackup to perform the full database backup on the instance of SQL Server.
                Log.NewLog(LogType.Info, "Start SQL Backup Procedure");
                sqlBackup.SqlBackup(sqlServer);

                ////Remove the backup device from the Backup object.    
                sqlBackup.Devices.Remove(deviceItem);

                Log.NewLog(LogType.Info, "Finish SQL Backup Task");

            }
            catch (Exception ex)
            {

                Log.NewLog(LogType.Error, "Error in SQL Backup Task. Exception: " + ex.Message);
                BackupError?.Invoke(null, ex.Message);

            }

        }

        /// <summary>
        /// SQLs the backup on complete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ServerMessageEventArgs"/> instance containing the event data.</param>
        private static void SqlBackupOnComplete(object sender, ServerMessageEventArgs e)
        {

            if (e.Error != null)
            {

                Log.NewLog(LogType.Info, e.Error.Message);
                BackupCompleted?.Invoke(null, e);

            }


        }

        /// <summary>
        /// SQLs the backup on information.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ServerMessageEventArgs"/> instance containing the event data.</param>
        private static void SqlBackupOnInformation(object sender, ServerMessageEventArgs e)
        {

            if (e.Error != null)
            {

                Log.NewLog(LogType.Info, e.Error.Message);

            }

        }

        /// <summary>
        /// SQLs the backup on percent complete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PercentCompleteEventArgs"/> instance containing the event data.</param>
        private static void SqlBackupOnPercentComplete(object sender, PercentCompleteEventArgs e)
        {

            if (e.Error != null)
            {

                BackupPercentage?.Invoke(null, e.Percent);

            }
            else
            {

                if (e.Error != null) BackupError?.Invoke(null, e.Error.Message);

            }

        }

    }

}
