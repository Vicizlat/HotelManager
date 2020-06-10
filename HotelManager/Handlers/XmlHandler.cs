using System.IO;
using System.Xml;

namespace HotelManager
{
    public static class XmlHandler
    {
        public static string GetXmlData(string xmlData)
        {
            using XmlReader reader = XmlReader.Create(Path.Combine(FileHandler.LocalPath, "Config.xml"));
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == xmlData)
                {
                    return reader.ReadString();
                }
            }
            return xmlData;
        }

        //public static void WriteConfigFile()
        //{
        //    XmlWriter xml = XmlWriter.Create($"{LocalPath}\\Config.xml");
        //    xml.WriteStartDocument();
        //    xml.WriteStartElement("Configuration");
        //    xml.WriteElementString("RemotePath", FtpPath);
        //    xml.WriteElementString("FtpUserName", UserName);
        //    xml.WriteElementString("FtpPassword", Password);
        //    xml.WriteEndElement();
        //    xml.WriteEndDocument();
        //    xml.Close();
        //}
    }
}