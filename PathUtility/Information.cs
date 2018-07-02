using System.IO;

namespace GeneralUtility.PathUtility
{
    public static class Information
    {

        /// <summary>
        /// Method to get the total free space in bytes
        /// </summary>
        /// <param name="driveName"> Drive letter Path</param>
        /// <returns>Return -1 if the driver is not found</returns>
        /// <example>GetFreeSpace("C:\")=</example>
        public static long GetFreeSpace(string driveName)
        {

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {

                    return drive.TotalFreeSpace;

                }
            }

            return -1;

        }

    }
}