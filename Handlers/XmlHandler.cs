using System.IO;
using System.Xml;

namespace Handlers
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
                    string abc = reader.ReadElementContentAsString();
                    return abc;
                }
            }
            return xmlData;
        }

        //public static void WriteConfigFile()
        //{
        //    XmlWriter xml = XmlWriter.Create($"{LocalPath}\\Config.xml");
        //    xml.WriteStartDocument();
        //    xml.WriteStartElement("Configuration");
        //    xml.WriteElementString("WebAddress", "shortWebPath");
        //    xml.WriteElementString("FtpAddress", "fullFtpPath");
        //    xml.WriteElementString("FtpUserName", "userName");
        //    xml.WriteElementString("FtpPassword", "password");
        //    xml.WriteEndElement();
        //    xml.WriteEndDocument();
        //    xml.Close();
        //}
    }
}