using System.Xml.Serialization;

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
    }
}