using System;
using System.IO;
using System.Xml.Serialization;
using HotelManager.Utils;

namespace HotelManager.Handlers
{
    public static class XmlHandler
    {
        public static XmlSerializer Serializer(object obj)
        {
            return new XmlSerializer(obj.GetType());
        }

        public static XmlSerializerNamespaces Namespaces(string prefix = "", string ns = "")
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(prefix, ns);
            return namespaces;
        }

        private static bool ReadSettingsFile()
        {
            StringReader reader = new StringReader(FileHandler.ReadAllText(Constants.SettingsFilename));
            return Settings.CreateSettings(XmlHandler.Serializer(Settings.Instance).Deserialize(reader));
        }

        private static void WriteSettingsFile()
        {
            StringWriter writer = new StringWriter();
            XmlHandler.Serializer(Settings.Instance).Serialize(writer, Settings.Instance, XmlHandler.Namespaces());
            FileHandler.WriteAllText(Constants.SettingsFilename, writer.ToString());
        }

        public static T GetFromXmlString<T>(T obj, string xmlString, string root)
        {
            StringReader reader = new StringReader(xmlString);
            return (T)new XmlSerializer(typeof(T), new XmlRootAttribute(root)).Deserialize(reader);
        }
    }
}