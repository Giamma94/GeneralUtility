using System;
using System.IO;
using System.Xml.Serialization;

namespace GeneralUtility.XmlUtility
{
    public static class XmlController
    {

        /// <summary>
        /// Get data from XML file to class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string filePath)
        {

            var ser = new XmlSerializer(typeof(T));

            using (var tr = new StreamReader(filePath))
            {

                return (T)ser.Deserialize(tr);

            }

        }


        /// <summary>
        /// Write data from class to XML file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <param name="filePath"></param>
        public static void SerializeToXml<T>(object Obj, string filePath)
        {

            using (var fileStream = new StreamWriter(filePath))
            {

                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(fileStream, Obj);

            }

        }

    }
}