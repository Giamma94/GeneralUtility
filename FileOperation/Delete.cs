using GeneralUtility.LogUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUtility.FileOperation
{
    /// <summary>
    /// Delete tools
    /// </summary>
    public static class Delete
    {
        /// <summary>
        /// Delete all file or directory in a specific folder
        /// </summary>
        /// <param name="FilePath">Path of the folder</param>
        public static void CleeanDirectory(string FilePath)
        {
            try
            {

                Log.NewLog(LogType.Warning, "Try to delete old backup");

                System.IO.DirectoryInfo di = new DirectoryInfo(FilePath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                    Log.NewLog(LogType.Info, "Delete file " + file.Name);
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                    Log.NewLog(LogType.Info, "Delete folder " + dir.Name);
                }
            }
            catch (Exception ex)
            {

                Log.NewLog(LogType.Error, "Delete old backup. Error: " + ex.Message);

            }

        }
    }
}
