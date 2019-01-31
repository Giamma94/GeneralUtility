using System.IO;
using System.Web.Script.Serialization;


namespace GeneralUtility.JsonUtility
{
    /// <summary>
    /// Serialize & Deserialize JSON
    /// </summary>
    public static class JsonController
    {
        /// <summary>
        /// Get data from JSON file to class
        /// </summary>
        /// <typeparam name="T">Type Of Class</typeparam>
        /// <param name="filePath">Path of the File</param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string filePath)
        {

            string json = File.ReadAllText(filePath);

            return new JavaScriptSerializer().Deserialize<T>(json);

        }


        /// <summary>
        /// Write data from class to JSON file
        /// </summary>
        /// <typeparam name="T">Type Of Class</typeparam>
        /// <param name="Obj">Data to serialize</param>
        /// <param name="filePath">Path of the File</param>
        public static void SerializeToJson<T>(object Obj, string filePath)
        {

            // Pass "Obj" object for conversion object to JSON string
            string json = new JavaScriptSerializer().Serialize((T)Obj);

            // Write that JSON to txt file
            File.WriteAllText(filePath, json);

        }
    }
}
