using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralUtility.FileOperation
{
    public static class Copy
    {

        /// <summary>
        /// Occurs when [data on copy].
        /// </summary>
        public static event EventHandler<OnCopy> DataOnCopy;
        /// <summary>
        /// The run task
        /// </summary>
        private static bool RunTask = true;
        /// <summary>
        /// The task a
        /// </summary>
        private static Task taskA;

        /// <summary>
        /// This method can help you to copy big file with slow performace impact 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="ByteFortransfer"></param>
        /// <param name="DelayForCicle"></param>
        public static void StartCopy(string inputFilePath, string outputFilePath, int ByteFortransfer, int DelayForCicle, int DelayOfEvent)
        {

            if(taskA != null)
            {

                taskA.Dispose();
                taskA = null;

            }

            taskA = new Task(() => CopyTask(inputFilePath, outputFilePath, ByteFortransfer, DelayForCicle, DelayOfEvent));
            taskA.Start();

        }

        /// <summary>
        /// Stops the copy.
        /// </summary>
        public static void StopCopy()
        {

            RunTask = false;

        }

        /// <summary>
        /// Async Task for Copy
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="ByteFortransfer"></param>
        /// <param name="DelayForCicle"></param>
        /// <returns></returns>
        private async static Task CopyTask(string inputFilePath, string outputFilePath, int ByteFortransfer, int DelayForCicle, int DelayOfEvent)
        {
            RunTask = true;
            int bytesRead;
            float TotalbytesRead = 0;
            float TotalFileBytes = 0;
            byte[] buffer = new byte[ByteFortransfer];

            try
            {
                using (FileStream fileStreamStart = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                {

                    using (FileStream fileStreamDest = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                    {

                        TotalFileBytes = fileStreamStart.Length;

                        if (DataOnCopy != null)
                        {
                            DataOnCopy(null,
                                new OnCopy()
                                {
                                    Percentage = 0,
                                    TotalbytesRead = 0,
                                    TotalFileBytes = 0,
                                    Error = null,
                                    Status = OnCopy.CopyStatus.Start
                                });

                            while ((bytesRead = fileStreamStart.Read(buffer, 0, buffer.Length)) > 0 && RunTask)
                            {
                                fileStreamDest.Write(buffer, 0, bytesRead);

                                TotalbytesRead = TotalbytesRead + bytesRead;

                                DataOnCopy(null,
                                    new OnCopy()
                                    {
                                        Percentage = (TotalbytesRead / fileStreamStart.Length) * 100,
                                        TotalbytesRead = TotalbytesRead,
                                        TotalFileBytes = TotalFileBytes,
                                        Error = null,
                                        Status = OnCopy.CopyStatus.Running
                                    });
                                Thread.Sleep(DelayForCicle);
                            }

                            RunTask = true;
                            Thread.Sleep(DelayOfEvent);
                            DataOnCopy(null,
                                new OnCopy()
                                {
                                    Percentage = 0,
                                    TotalbytesRead = 0,
                                    TotalFileBytes = 0,
                                    Error = null,
                                    Status = OnCopy.CopyStatus.Finish
                                });
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                RunTask = true;
                Thread.Sleep(DelayOfEvent);
                if (DataOnCopy != null)
                    DataOnCopy(null,
                        new OnCopy()
                        {
                            Percentage = 0,
                            TotalbytesRead = 0,
                            TotalFileBytes = 0,
                            Error = ex.Message + " - " + ex.TargetSite.Name,
                            Status = OnCopy.CopyStatus.Error
                        });
            }

            
        }

    }

    /// <summary>
    /// Class Event Data
    /// </summary>
    public class OnCopy : EventArgs
    {
        /// <summary>
        /// Status of the Copy Task
        /// </summary>
        public enum CopyStatus
        {
            Start,
            Stop,
            Running,
            Finish,
            Error
        }

        public float Percentage { get; set; }
        public float TotalbytesRead { get; set; }
        public float TotalFileBytes { get; set; }
        public CopyStatus Status { get; set; }
        public string Error { get; set; }

    }
}
