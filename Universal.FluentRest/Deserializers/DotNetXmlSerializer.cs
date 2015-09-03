using System.IO;
using System.Xml.Serialization;

namespace Universal.FluentRest.Deserializers
{
    public class DotNetXmlSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var writer = new StringWriter();
            serializer.Serialize(writer, obj);

            return writer.ToString();
        }
    }
}