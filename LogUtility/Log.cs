
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralUtility.LogUtility
{

    public enum LogType
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public static class Log
    {

        /// <summary>
        /// The log stack
        /// </summary>
        private static List<LogDTO> LogStack = new List<LogDTO>();

        /// <summary>
        /// The task a
        /// </summary>
        private static Task taskA;
        /// <summary>
        /// The log task run
        /// </summary>
        private static bool LogTaskRun;

        /// <summary>
        /// Starts the log utility.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="DayOfLog">The day of log.</param>
        /// <param name="HourOfDeleting">The hour of deleting.</param>
        public static void StartLogUtility(string path, string filename, int DayOfLog, int HourOfDeleting)
        {

            LogStack.Clear();
            LogTaskRun = true;

            taskA = new Task(() => LogManager_Task(path, filename, DayOfLog, HourOfDeleting));
            taskA.Start();

        }


        /// <summary>
        /// Logs the manager task.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dayOfLog">The day of log.</param>
        /// <param name="hourOfDeleting">The hour of deleting.</param>
        private async static void LogManager_Task(string path, string fileName, int dayOfLog, int hourOfDeleting)
        {

            DateTime dd = DateTime.Now;

            while (LogTaskRun)
            {

                if (LogStack.Count > 0)
                {

                    foreach (var value in (from a in LogStack group a by a.LogTime.Date into s select new {DatTime = s.Key, Data = s.ToList()}))
                    {

                        List<string> app = new List<string>();
                        foreach (LogDTO lg in value.Data)
                        {
                            app.Add( lg.LogTime + ";" + lg.TypeOfLog + ";" + lg.Message);
                        }

                        File.AppendAllLines(Path.Combine(path, fileName + value.DatTime.Day + value.DatTime.Month + value.DatTime.Year + ".txt"), app);
                        LogStack.Clear();
                    }

                }

                if (DateTime.Now > dd)
                {
                    dd = DateTime.Now.AddHours(hourOfDeleting);

                    List<FileInfo> _infoFiles = new List<FileInfo>();
                    foreach (string item in Directory.GetFiles(path))
                    {

                        _infoFiles.Add(new FileInfo(item));

                    }

                    foreach (FileInfo fi in (from a in _infoFiles where (DateTime.Now - a.CreationTime).TotalDays >= dayOfLog select a))
                    {
                        File.Delete(Path.Combine(fi.DirectoryName, fi.Name));

                        NewLog(LogType.Info, "Deleted file log: " + fi.Name);

                    }
                }

            }

        }

        /// <summary>
        /// News the log.
        /// </summary>
        /// <param name="lgType">Type of the lg.</param>
        /// <param name="message">The message.</param>
        public static void NewLog(LogType lgType, string message)
        {
            if (LogTaskRun)
            {

                LogDTO appLog = new LogDTO { LogTime = DateTime.Now, TypeOfLog = lgType, Message = message };

                LogStack.Add(appLog);

            }
            

        }

        /// <summary>
        /// News the log.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="lgType">Type of the lg.</param>
        /// <param name="message">The message.</param>
        public static void NewLog(DateTime dt ,LogType lgType, string message)
        {
            if (LogTaskRun)
            {

                LogDTO appLog = new LogDTO { LogTime = dt, TypeOfLog = lgType, Message = message };

                LogStack.Add(appLog);

            }

        }

    }

    public class LogDTO
    {

        public virtual DateTime LogTime { get; set; }
        public virtual LogType TypeOfLog { get; set; }
        public virtual string Message { get; set; }

    }

}
